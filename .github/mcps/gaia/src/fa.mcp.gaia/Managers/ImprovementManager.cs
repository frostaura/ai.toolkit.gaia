using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text.Json;
using FrostAura.MCP.Gaia.Models;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers
{
    /// <summary>
    /// Improvement Manager - Tools for agents to log runtime frustrations and enhancement opportunities
    /// Enables agents to "wish improvements into existence" by documenting what would help them work better
    /// Persists to docs/improvements.json for review and implementation
    /// </summary>
    [McpServerToolType]
    public class ImprovementManager
    {
        private readonly ILogger<ImprovementManager> _logger;
        private const string ImprovementsPath = "docs/improvements.json";
        private static readonly SemaphoreSlim _fileSemaphore = new(1, 1);

        // Thread-safe in-memory storage - persisted to disk
        private static readonly ConcurrentDictionary<string, GaiaImprovement> _improvements = new();

        // Write queue for batching persistence operations
        private static BlockingCollection<WriteRequest>? _writeQueue = new(new ConcurrentQueue<WriteRequest>());
        private static Task? _writeProcessorTask;
        private static CancellationTokenSource? _cancellationTokenSource = new();
        private static readonly object _processorLock = new();
        private static bool _processorStarted = false;

        // Track pending writes for acknowledgment
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingWrites = new();

        private sealed class WriteRequest
        {
            public required string RequestId { get; init; }
            public required GaiaImprovement Improvement { get; init; }
        }

        public ImprovementManager(ILogger<ImprovementManager> logger)
        {
            _logger = logger;

            // Load improvements from disk
            LoadImprovementsAsync().Wait();

            // Start the background write processor (only once across all instances)
            StartWriteProcessor();

            _logger.LogInformation(
                "[STARTUP] ImprovementManager initialized | ImprovementCount={Count} | WriteQueueEnabled=true",
                _improvements.Count);
        }

        /// <summary>
        /// Start the background write processor task
        /// </summary>
        private void StartWriteProcessor()
        {
            lock (_processorLock)
            {
                if (_processorStarted) return;

                // Reinitialize if previously disposed
                if (_writeQueue == null || _writeQueue.IsAddingCompleted)
                {
                    _writeQueue = new BlockingCollection<WriteRequest>(new ConcurrentQueue<WriteRequest>());
                }
                if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                _processorStarted = true;

                _writeProcessorTask = Task.Run(async () =>
                {
                    _logger.LogInformation("[WRITE_QUEUE] Background write processor started (improvements)");

                    try
                    {
                        await ProcessWriteQueueAsync(_cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("[WRITE_QUEUE] Background write processor stopped (improvements, cancelled)");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[WRITE_QUEUE] Background write processor failed (improvements) | Error={ErrorMessage}", ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Process the write queue - batches writes for efficiency
        /// </summary>
        private async Task ProcessWriteQueueAsync(CancellationToken cancellationToken)
        {
            var batchDelay = TimeSpan.FromMilliseconds(100);
            var completedRequestIds = new List<string>();

            while (!cancellationToken.IsCancellationRequested && _writeQueue != null)
            {
                try
                {
                    // Wait for at least one item
                    if (_writeQueue == null || !_writeQueue.TryTake(out var firstRequest, Timeout.Infinite, cancellationToken))
                        continue;

                    completedRequestIds.Clear();
                    completedRequestIds.Add(firstRequest.RequestId);

                    // Apply the first request to in-memory storage
                    _improvements[firstRequest.Improvement.Id] = firstRequest.Improvement;

                    // Wait briefly to batch more writes
                    await Task.Delay(batchDelay, cancellationToken);

                    // Drain any additional pending writes
                    while (_writeQueue != null && _writeQueue.TryTake(out var additionalRequest))
                    {
                        completedRequestIds.Add(additionalRequest.RequestId);
                        _improvements[additionalRequest.Improvement.Id] = additionalRequest.Improvement;
                    }

                    // Perform single batched write to disk
                    await SaveImprovementsToDiskAsync();

                    _logger.LogDebug(
                        "[WRITE_QUEUE] Batch completed (improvements) | BatchSize={BatchSize} | TotalImprovements={TotalImprovements}",
                        completedRequestIds.Count,
                        _improvements.Count);

                    // Signal completion to all waiting callers
                    foreach (var requestId in completedRequestIds)
                    {
                        if (_pendingWrites.TryRemove(requestId, out var tcs))
                        {
                            tcs.TrySetResult(true);
                        }
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Flush remaining items before exit
                    await FlushRemainingWritesAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[WRITE_QUEUE] Error processing batch (improvements) | Error={ErrorMessage}", ex.Message);

                    // Signal failure to waiting callers
                    foreach (var requestId in completedRequestIds)
                    {
                        if (_pendingWrites.TryRemove(requestId, out var tcs))
                        {
                            tcs.TrySetException(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Flush any remaining writes in the queue before shutdown
        /// </summary>
        private async Task FlushRemainingWritesAsync()
        {
            var remaining = new List<WriteRequest>();

            while (_writeQueue != null && _writeQueue.TryTake(out var request))
            {
                remaining.Add(request);
                _improvements[request.Improvement.Id] = request.Improvement;
            }

            if (remaining.Count > 0)
            {
                _logger.LogInformation("[WRITE_QUEUE] Flushing {Count} remaining writes before shutdown (improvements)", remaining.Count);
                await SaveImprovementsToDiskAsync();

                foreach (var request in remaining)
                {
                    if (_pendingWrites.TryRemove(request.RequestId, out var tcs))
                    {
                        tcs.TrySetResult(true);
                    }
                }
            }
        }

        /// <summary>
        /// Queue an improvement write operation
        /// </summary>
        private Task QueueWriteAsync(GaiaImprovement improvement)
        {
            // Check if queue unavailable - save synchronously as fallback
            if (_writeQueue == null || _writeQueue.IsAddingCompleted)
            {
                _logger.LogWarning("[WRITE_QUEUE] Queue unavailable, saving synchronously | Id={Id}", improvement.Id);
                return SaveImprovementsToDiskAsync();
            }

            var requestId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            _pendingWrites[requestId] = tcs;

            var request = new WriteRequest
            {
                RequestId = requestId,
                Improvement = improvement
            };

            try
            {
                if (!_writeQueue.TryAdd(request))
                {
                    _pendingWrites.TryRemove(requestId, out _);
                    _logger.LogWarning("[WRITE_QUEUE] Failed to queue, saving synchronously | Id={Id}", improvement.Id);
                    return SaveImprovementsToDiskAsync();
                }
            }
            catch (ObjectDisposedException)
            {
                _pendingWrites.TryRemove(requestId, out _);
                _logger.LogWarning("[WRITE_QUEUE] Queue disposed, saving synchronously | Id={Id}", improvement.Id);
                return SaveImprovementsToDiskAsync();
            }

            _logger.LogDebug(
                "[WRITE_QUEUE] Queued (improvement) | RequestId={RequestId} | Id={Id} | QueueSize={QueueSize}",
                requestId,
                improvement.Id,
                _writeQueue.Count);

            return tcs.Task;
        }

        /// <summary>
        /// Load improvements from disk
        /// </summary>
        private async Task LoadImprovementsAsync()
        {
            try
            {
                if (!File.Exists(ImprovementsPath))
                {
                    _logger.LogDebug("[IMPROVEMENT:LOAD] No improvements file found at {Path}, creating empty file", ImprovementsPath);

                    // Create the directory and empty file if it doesn't exist
                    var directory = Path.GetDirectoryName(ImprovementsPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Create an empty JSON array file
                    await File.WriteAllTextAsync(ImprovementsPath, "[]");
                    _logger.LogInformation("[IMPROVEMENT:LOAD] Created new improvements file at {Path}", ImprovementsPath);
                    return;
                }

                await _fileSemaphore.WaitAsync();
                try
                {
                    var json = await File.ReadAllTextAsync(ImprovementsPath);
                    var improvements = JsonSerializer.Deserialize<List<GaiaImprovement>>(json);

                    if (improvements != null)
                    {
                        _improvements.Clear();
                        foreach (var improvement in improvements)
                        {
                            _improvements[improvement.Id] = improvement;
                        }

                        _logger.LogInformation(
                            "[IMPROVEMENT:LOAD] Loaded {Count} improvements from {Path}",
                            improvements.Count,
                            ImprovementsPath);
                    }
                }
                finally
                {
                    _fileSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMPROVEMENT:LOAD] Failed to load improvements | Error={ErrorMessage}", ex.Message);
            }
        }

        /// <summary>
        /// Save improvements to disk (called only by write processor)
        /// </summary>
        private async Task SaveImprovementsToDiskAsync()
        {
            try
            {
                await _fileSemaphore.WaitAsync();
                try
                {
                    var directory = Path.GetDirectoryName(ImprovementsPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var improvements = _improvements.Values
                        .OrderByDescending(i => i.Priority)
                        .ThenByDescending(i => i.Created)
                        .ToList();

                    var json = JsonSerializer.Serialize(improvements, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await File.WriteAllTextAsync(ImprovementsPath, json);

                    _logger.LogDebug(
                        "[IMPROVEMENT:SAVE] Saved {Count} improvements to {Path}",
                        improvements.Count,
                        ImprovementsPath);
                }
                finally
                {
                    _fileSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMPROVEMENT:SAVE] Failed to save improvements | Error={ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Log an improvement request - agents use this to wish enhancements into existence
        /// </summary>
        [McpServerTool]
        [Description("Log an improvement request when you encounter frustrations, missing capabilities, or workflow inefficiencies. Use this to wish improvements into existence!")]
        public Task<LogImprovementResponse> log_improvement(
            [Description("The improvement request to log")] LogImprovementRequest request)
        {
            _logger.LogDebug(
                "[IMPROVEMENT:LOG] Starting | Agent={Agent} | Type={Type} | Title={Title}",
                request.Agent,
                request.Type,
                request.Title);

            try
            {
                var improvement = new GaiaImprovement
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = request.Type,
                    Agent = request.Agent,
                    ProjectName = request.ProjectName,
                    Title = request.Title,
                    Description = request.Description,
                    Context = request.Context,
                    Suggestion = request.Suggestion,
                    Priority = request.Priority,
                    Status = "Logged",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                // Immediately update in-memory for read consistency
                _improvements[improvement.Id] = improvement;

                // Queue for disk persistence (fire-and-forget with guaranteed delivery)
                _ = QueueWriteAsync(improvement);

                _logger.LogInformation(
                    "[IMPROVEMENT:LOGGED] Agent={Agent} | Type={Type} | Priority={Priority} | Title={Title} | TotalImprovements={TotalImprovements}",
                    request.Agent,
                    request.Type,
                    request.Priority,
                    request.Title,
                    _improvements.Count);

                return Task.FromResult(new LogImprovementResponse
                {
                    Success = true,
                    Message = $"Improvement logged successfully: {request.Title}. Your feedback helps evolve the system!",
                    Improvement = improvement
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[IMPROVEMENT:LOG] Failed | Agent={Agent} | Type={Type} | Error={ErrorMessage}",
                    request.Agent,
                    request.Type,
                    ex.Message);

                return Task.FromResult(new LogImprovementResponse
                {
                    Success = false,
                    Message = $"Error logging improvement: {ex.Message}",
                    Improvement = null
                });
            }
        }

        /// <summary>
        /// Get improvements with optional filtering
        /// </summary>
        [McpServerTool]
        [Description("Get logged improvements with optional filtering by status, type, or agent")]
        public Task<ReadImprovementsResponse> read_improvements(
            [Description("Filter by status (e.g., Logged, UnderReview, Planned, Implemented, Dismissed)")] string? status = null,
            [Description("Filter by improvement type")] ImprovementType? type = null,
            [Description("Filter by agent name")] string? agent = null)
        {
            _logger.LogDebug(
                "[IMPROVEMENT:READ] Starting | Status={Status} | Type={Type} | Agent={Agent}",
                status ?? "(all)",
                type?.ToString() ?? "(all)",
                agent ?? "(all)");

            try
            {
                var improvements = _improvements.Values.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(status))
                {
                    improvements = improvements.Where(i => i.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                }

                if (type.HasValue)
                {
                    improvements = improvements.Where(i => i.Type == type.Value);
                }

                if (!string.IsNullOrWhiteSpace(agent))
                {
                    improvements = improvements.Where(i => i.Agent.Equals(agent, StringComparison.OrdinalIgnoreCase));
                }

                var improvementList = improvements
                    .OrderByDescending(i => i.Priority)
                    .ThenByDescending(i => i.Created)
                    .ToList();

                var filterParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(status)) filterParts.Add($"status={status}");
                if (type.HasValue) filterParts.Add($"type={type}");
                if (!string.IsNullOrWhiteSpace(agent)) filterParts.Add($"agent={agent}");

                var filterDescription = filterParts.Count > 0 ? string.Join(", ", filterParts) : "all improvements";

                var response = new ReadImprovementsResponse
                {
                    Summary = $"{improvementList.Count} improvements ({filterDescription})",
                    Filter = filterDescription,
                    Count = improvementList.Count,
                    Improvements = improvementList
                };

                _logger.LogInformation(
                    "[IMPROVEMENT:READ] Completed | Filter={Filter} | TotalInStore={TotalImprovements} | Returned={ReturnedCount}",
                    filterDescription,
                    _improvements.Count,
                    improvementList.Count);

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMPROVEMENT:READ] Failed | Error={ErrorMessage}", ex.Message);

                return Task.FromResult(new ReadImprovementsResponse
                {
                    Summary = $"Error: {ex.Message}",
                    Filter = "error",
                    Count = 0,
                    Improvements = new List<GaiaImprovement>()
                });
            }
        }

        /// <summary>
        /// Clear all improvements (useful for starting fresh)
        /// </summary>
        [McpServerTool]
        [Description("Clear all improvements from storage (useful for starting fresh)")]
        public async Task<ClearResponse> clear_improvements()
        {
            var count = _improvements.Count;
            _improvements.Clear();

            // Clear the file directly (bypass queue for immediate effect)
            await SaveImprovementsToDiskAsync();

            _logger.LogWarning(
                "[IMPROVEMENT:CLEAR] All improvements cleared | ClearedCount={ClearedCount}",
                count);

            return new ClearResponse
            {
                Success = true,
                Message = $"Cleared {count} improvements",
                ClearedCount = count
            };
        }

    }
}
