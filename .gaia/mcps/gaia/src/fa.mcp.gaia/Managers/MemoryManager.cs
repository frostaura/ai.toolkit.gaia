using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers
{
    /// <summary>
    /// Memory Manager - Essential tools for session-level and persistent memory management
    /// Supports both ephemeral (SessionLength) and permanent (ProjectWide) storage
    /// Uses a write queue to ensure no memories are lost when file is locked
    /// </summary>
    [McpServerToolType]
    public class MemoryManager : IDisposable
    {
        private readonly ILogger<MemoryManager> _logger;
        private const string PersistentMemoryPath = ".gaia/memory.json";
        private static readonly SemaphoreSlim _fileSemaphore = new(1, 1);

        // Thread-safe in-memory storage - session-level memories (lost on service restart)
        private static readonly ConcurrentDictionary<string, GaiaMemory> _sessionMemories = new();

        // Thread-safe in-memory storage - project-wide memories (persisted to disk)
        private static readonly ConcurrentDictionary<string, GaiaMemory> _persistentMemories = new();

        // Write queue for batching persistence operations
        private static BlockingCollection<WriteRequest>? _writeQueue = new(new ConcurrentQueue<WriteRequest>());
        private static Task? _writeProcessorTask;
        private static CancellationTokenSource? _cancellationTokenSource = new();
        private static readonly object _processorLock = new();
        private static bool _processorStarted = false;
        private static bool _disposed = false;

        // Track pending writes for acknowledgment
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingWrites = new();

        private sealed class WriteRequest
        {
            public required string RequestId { get; init; }
            public required GaiaMemory Memory { get; init; }
            public required bool IsDelete { get; init; }
        }

        public MemoryManager(ILogger<MemoryManager> logger)
        {
            _logger = logger;

            // Load persistent memories from disk
            LoadPersistentMemoriesAsync().Wait();

            // Start the background write processor (only once across all instances)
            StartWriteProcessor();

            _logger.LogInformation(
                "[STARTUP] MemoryManager initialized | SessionMemories={SessionCount} | PersistentMemories={PersistentCount} | WriteQueueEnabled=true",
                _sessionMemories.Count,
                _persistentMemories.Count);
        }

        /// <summary>
        /// Start the background write processor task
        /// </summary>
        private void StartWriteProcessor()
        {
            lock (_processorLock)
            {
                if (_processorStarted || _disposed) return;

                // Reinitialize if previously disposed
                if (_writeQueue == null || _writeQueue.IsAddingCompleted)
                {
                    _writeQueue = new BlockingCollection<WriteRequest>(new ConcurrentQueue<WriteRequest>());
                }
                if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                _disposed = false;
                _processorStarted = true;

                _writeProcessorTask = Task.Run(async () =>
                {
                    _logger.LogInformation("[WRITE_QUEUE] Background write processor started");

                    try
                    {
                        await ProcessWriteQueueAsync(_cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("[WRITE_QUEUE] Background write processor stopped (cancelled)");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[WRITE_QUEUE] Background write processor failed | Error={ErrorMessage}", ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Process the write queue - batches writes for efficiency
        /// </summary>
        private async Task ProcessWriteQueueAsync(CancellationToken cancellationToken)
        {
            var batchDelay = TimeSpan.FromMilliseconds(100); // Batch writes within 100ms window
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
                    if (firstRequest.IsDelete)
                    {
                        _persistentMemories.TryRemove(firstRequest.Memory.CompositeKey, out _);
                    }
                    else
                    {
                        _persistentMemories[firstRequest.Memory.CompositeKey] = firstRequest.Memory;
                    }

                    // Wait briefly to batch more writes
                    await Task.Delay(batchDelay, cancellationToken);

                    // Drain any additional pending writes
                    while (_writeQueue != null && _writeQueue.TryTake(out var additionalRequest))
                    {
                        completedRequestIds.Add(additionalRequest.RequestId);

                        if (additionalRequest.IsDelete)
                        {
                            _persistentMemories.TryRemove(additionalRequest.Memory.CompositeKey, out _);
                        }
                        else
                        {
                            _persistentMemories[additionalRequest.Memory.CompositeKey] = additionalRequest.Memory;
                        }
                    }

                    // Perform single batched write to disk
                    await SavePersistentMemoriesToDiskAsync();

                    _logger.LogDebug(
                        "[WRITE_QUEUE] Batch completed | BatchSize={BatchSize} | TotalMemories={TotalMemories}",
                        completedRequestIds.Count,
                        _persistentMemories.Count);

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
                    _logger.LogError(ex, "[WRITE_QUEUE] Error processing batch | Error={ErrorMessage}", ex.Message);

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

                if (request.IsDelete)
                {
                    _persistentMemories.TryRemove(request.Memory.CompositeKey, out _);
                }
                else
                {
                    _persistentMemories[request.Memory.CompositeKey] = request.Memory;
                }
            }

            if (remaining.Count > 0)
            {
                _logger.LogInformation("[WRITE_QUEUE] Flushing {Count} remaining writes before shutdown", remaining.Count);
                await SavePersistentMemoriesToDiskAsync();

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
        /// Queue a memory write operation
        /// </summary>
        private Task QueueWriteAsync(GaiaMemory memory, bool isDelete = false)
        {
            // Check if disposed or queue unavailable - save synchronously as fallback
            if (_disposed || _writeQueue == null || _writeQueue.IsAddingCompleted)
            {
                _logger.LogWarning("[WRITE_QUEUE] Queue unavailable, saving synchronously | Key={Key}", memory.CompositeKey);
                return SavePersistentMemoriesToDiskAsync();
            }

            var requestId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            _pendingWrites[requestId] = tcs;

            var request = new WriteRequest
            {
                RequestId = requestId,
                Memory = memory,
                IsDelete = isDelete
            };

            try
            {
                if (!_writeQueue.TryAdd(request))
                {
                    _pendingWrites.TryRemove(requestId, out _);
                    _logger.LogWarning("[WRITE_QUEUE] Failed to queue, saving synchronously | Key={Key}", memory.CompositeKey);
                    return SavePersistentMemoriesToDiskAsync();
                }
            }
            catch (ObjectDisposedException)
            {
                _pendingWrites.TryRemove(requestId, out _);
                _logger.LogWarning("[WRITE_QUEUE] Queue disposed, saving synchronously | Key={Key}", memory.CompositeKey);
                return SavePersistentMemoriesToDiskAsync();
            }

            _logger.LogDebug(
                "[WRITE_QUEUE] Queued | RequestId={RequestId} | Key={Key} | QueueSize={QueueSize}",
                requestId,
                memory.CompositeKey,
                _writeQueue.Count);

            return tcs.Task;
        }

        /// <summary>
        /// Load persistent memories from disk
        /// </summary>
        private async Task LoadPersistentMemoriesAsync()
        {
            try
            {
                if (!File.Exists(PersistentMemoryPath))
                {
                    _logger.LogDebug("[MEMORY:LOAD] No persistent memory file found at {Path}, creating empty file", PersistentMemoryPath);

                    // Create the directory and empty file if it doesn't exist
                    var directory = Path.GetDirectoryName(PersistentMemoryPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Create an empty JSON array file
                    await File.WriteAllTextAsync(PersistentMemoryPath, "[]");
                    _logger.LogInformation("[MEMORY:LOAD] Created new persistent memory file at {Path}", PersistentMemoryPath);
                    return;
                }

                await _fileSemaphore.WaitAsync();
                try
                {
                    var json = await File.ReadAllTextAsync(PersistentMemoryPath);
                    var memories = JsonSerializer.Deserialize<List<GaiaMemory>>(json);

                    if (memories != null)
                    {
                        _persistentMemories.Clear();
                        foreach (var memory in memories)
                        {
                            _persistentMemories[memory.CompositeKey] = memory;
                        }

                        _logger.LogInformation(
                            "[MEMORY:LOAD] Loaded {Count} persistent memories from {Path}",
                            memories.Count,
                            PersistentMemoryPath);
                    }
                }
                finally
                {
                    _fileSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MEMORY:LOAD] Failed to load persistent memories | Error={ErrorMessage}", ex.Message);
            }
        }

        /// <summary>
        /// Save persistent memories to disk (called only by write processor)
        /// </summary>
        private async Task SavePersistentMemoriesToDiskAsync()
        {
            try
            {
                await _fileSemaphore.WaitAsync();
                try
                {
                    var directory = Path.GetDirectoryName(PersistentMemoryPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var memories = _persistentMemories.Values.OrderBy(m => m.Category).ThenBy(m => m.Key).ToList();
                    var json = JsonSerializer.Serialize(memories, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await File.WriteAllTextAsync(PersistentMemoryPath, json);

                    _logger.LogDebug(
                        "[MEMORY:SAVE] Saved {Count} persistent memories to {Path}",
                        memories.Count,
                        PersistentMemoryPath);
                }
                finally
                {
                    _fileSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MEMORY:SAVE] Failed to save persistent memories | Error={ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Store important decisions/context for later recalling (upserts by category+key)
        /// </summary>
        [McpServerTool]
        [Description("Store important decisions/context for later recalling. Upserts by category+key to prevent duplicates.")]
        public Task<RememberResponse> remember(
            [Description("The memory to store")] RememberRequest request)
        {
            _logger.LogDebug(
                "[MEMORY:STORE] Starting | Category={Category} | Key={Key} | Duration={Duration} | ValueLength={ValueLength}",
                request.Category,
                request.Key,
                request.Duration,
                request.Value?.Length ?? 0);

            try
            {
                var memory = new GaiaMemory
                {
                    Category = request.Category,
                    Key = request.Key,
                    Value = request.Value ?? string.Empty,
                    Duration = request.Duration,
                    Updated = DateTime.UtcNow
                };
                var compositeKey = memory.CompositeKey;

                // Select appropriate storage based on duration
                var targetStorage = request.Duration == MemoryDuration.ProjectWide
                    ? _persistentMemories
                    : _sessionMemories;

                var isUpdate = targetStorage.TryGetValue(compositeKey, out var existingMemory);

                if (isUpdate && existingMemory != null)
                {
                    memory.Created = existingMemory.Created;
                }
                else
                {
                    memory.Created = DateTime.UtcNow;
                }

                // For session memories, update immediately
                if (request.Duration == MemoryDuration.SessionLength)
                {
                    targetStorage[compositeKey] = memory;
                }
                else
                {
                    // For persistent memories, queue the write (memory is applied in the queue processor)
                    // Immediately update in-memory for read consistency
                    _persistentMemories[compositeKey] = memory;

                    // Queue for disk persistence (fire-and-forget with guaranteed delivery)
                    _ = QueueWriteAsync(memory, isDelete: false);
                }

                if (isUpdate)
                {
                    _logger.LogInformation(
                        "[MEMORY:UPDATED] Category={Category} | Key={Key} | Duration={Duration} | ValueLength={ValueLength} | Session={SessionCount} | Persistent={PersistentCount}",
                        request.Category,
                        request.Key,
                        request.Duration,
                        request.Value?.Length ?? 0,
                        _sessionMemories.Count,
                        _persistentMemories.Count);
                }
                else
                {
                    _logger.LogInformation(
                        "[MEMORY:STORED] Category={Category} | Key={Key} | Duration={Duration} | ValueLength={ValueLength} | Session={SessionCount} | Persistent={PersistentCount}",
                        request.Category,
                        request.Key,
                        request.Duration,
                        request.Value?.Length ?? 0,
                        _sessionMemories.Count,
                        _persistentMemories.Count);
                }

                return Task.FromResult(new RememberResponse
                {
                    Success = true,
                    Message = $"Memory {(isUpdate ? "updated" : "stored")} ({request.Duration}): {request.Category}/{request.Key}",
                    WasUpdate = isUpdate,
                    Memory = memory
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MEMORY:STORE] Failed | Category={Category} | Key={Key} | Error={ErrorMessage}",
                    request.Category,
                    request.Key,
                    ex.Message);

                return Task.FromResult(new RememberResponse
                {
                    Success = false,
                    Message = $"Error storing memory: {ex.Message}",
                    WasUpdate = false,
                    Memory = null
                });
            }
        }

        /// <summary>
        /// Search previous decisions/context with fuzzy matching
        /// Aggregates results from both session-level and persistent memories
        /// </summary>
        [McpServerTool]
        [Description("Search previous decisions/context with fuzzy matching")]
        public Task<RecallResponse> recall(
            [Description("The search request")] RecallRequest request)
        {
            var totalMemories = _sessionMemories.Count + _persistentMemories.Count;

            _logger.LogDebug(
                "[MEMORY:RECALL] Starting | Query={Query} | MaxResults={MaxResults} | Session={SessionCount} | Persistent={PersistentCount}",
                request.Query,
                request.MaxResults,
                _sessionMemories.Count,
                _persistentMemories.Count);

            try
            {
                if (totalMemories == 0)
                {
                    _logger.LogInformation("[MEMORY:RECALL] No memories in store | Query={Query}", request.Query);

                    return Task.FromResult(new RecallResponse
                    {
                        Count = 0,
                        TotalMatches = 0,
                        Query = request.Query,
                        Message = "No memories found. Use remember() to store memories first.",
                        Results = new List<MemorySearchResult>()
                    });
                }

                var scoredResults = new List<(GaiaMemory memory, double score)>();
                var queryLower = request.Query.ToLowerInvariant();
                var queryWords = queryLower.Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);

                // Aggregate all memories from both storages
                var allMemories = _sessionMemories.Values.Concat(_persistentMemories.Values);

                foreach (var memory in allMemories)
                {
                    var content = $"{memory.Category} {memory.Key} {memory.Value}".ToLowerInvariant();
                    double score = 0;

                    // Exact match (highest score)
                    if (content.Contains(queryLower))
                    {
                        score = 100;
                    }
                    else
                    {
                        // Fuzzy matching - check how many query words are found
                        int wordsFound = 0;
                        int totalPositionScore = 0;

                        foreach (var word in queryWords)
                        {
                            var index = content.IndexOf(word, StringComparison.OrdinalIgnoreCase);
                            if (index >= 0)
                            {
                                wordsFound++;
                                totalPositionScore += Math.Max(0, 100 - (index / 10));
                            }
                        }

                        if (wordsFound > 0)
                        {
                            score = (wordsFound * 60.0 / queryWords.Length) + (totalPositionScore / queryWords.Length * 0.4);

                            // Bonus for category/key matches
                            if (memory.Category.Contains(queryLower, StringComparison.OrdinalIgnoreCase))
                                score += 20;
                            if (memory.Key.Contains(queryLower, StringComparison.OrdinalIgnoreCase))
                                score += 15;
                        }
                    }

                    if (score > 0)
                    {
                        scoredResults.Add((memory, score));
                    }
                }

                if (scoredResults.Count == 0)
                {
                    _logger.LogInformation(
                        "[MEMORY:RECALL] No matches | Query={Query} | SearchedMemories={SearchedCount}",
                        request.Query,
                        totalMemories);

                    return Task.FromResult(new RecallResponse
                    {
                        Count = 0,
                        TotalMatches = 0,
                        Query = request.Query,
                        Message = $"No memories found matching '{request.Query}'",
                        Results = new List<MemorySearchResult>()
                    });
                }

                var topResults = scoredResults
                    .OrderByDescending(r => r.score)
                    .Take(request.MaxResults)
                    .Select(r => new MemorySearchResult
                    {
                        Memory = r.memory,
                        Relevance = Math.Round(r.score, 1)
                    })
                    .ToList();

                _logger.LogInformation(
                    "[MEMORY:RECALL] Completed | Query={Query} | TotalMatches={TotalMatches} | Returned={ReturnedCount} | TopScore={TopScore}",
                    request.Query,
                    scoredResults.Count,
                    topResults.Count,
                    topResults.FirstOrDefault()?.Relevance ?? 0);

                return Task.FromResult(new RecallResponse
                {
                    Count = topResults.Count,
                    TotalMatches = scoredResults.Count,
                    Query = request.Query,
                    Results = topResults
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MEMORY:RECALL] Failed | Query={Query} | Error={ErrorMessage}",
                    request.Query,
                    ex.Message);

                return Task.FromResult(new RecallResponse
                {
                    Count = 0,
                    TotalMatches = 0,
                    Query = request.Query,
                    Message = $"Error recalling memories: {ex.Message}",
                    Results = new List<MemorySearchResult>()
                });
            }
        }

        /// <summary>
        /// Clear all memories (useful for starting fresh)
        /// </summary>
        [McpServerTool]
        [Description("Clear all memories from memory (useful for starting fresh)")]
        public async Task<ClearResponse> clear_memories()
        {
            var sessionCount = _sessionMemories.Count;
            var persistentCount = _persistentMemories.Count;
            var totalCount = sessionCount + persistentCount;

            _sessionMemories.Clear();
            _persistentMemories.Clear();

            // Clear the persistent file directly (bypass queue for immediate effect)
            await SavePersistentMemoriesToDiskAsync();

            _logger.LogWarning(
                "[MEMORY:CLEAR] All memories cleared | SessionCleared={SessionCount} | PersistentCleared={PersistentCount} | TotalCleared={TotalCount}",
                sessionCount,
                persistentCount,
                totalCount);

            return new ClearResponse
            {
                Success = true,
                Message = $"Cleared {totalCount} memories (Session: {sessionCount}, Persistent: {persistentCount})",
                ClearedCount = totalCount
            };
        }

        /// <summary>
        /// Dispose and flush any pending writes
        /// </summary>
        public void Dispose()
        {
            lock (_processorLock)
            {
                if (_disposed) return;
                _disposed = true;
                _processorStarted = false;
            }

            _logger.LogInformation("[MEMORY:DISPOSE] Disposing MemoryManager, flushing pending writes");

            try
            {
                _cancellationTokenSource?.Cancel();

                // Wait for write processor to complete (with timeout)
                if (_writeProcessorTask != null)
                {
                    _writeProcessorTask.Wait(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[MEMORY:DISPOSE] Error during disposal | Error={ErrorMessage}", ex.Message);
            }
            finally
            {
                try
                {
                    _writeQueue?.CompleteAdding();
                }
                catch { /* Ignore if already completed */ }
            }
        }
    }
}
