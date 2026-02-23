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
        private const string ProjectsBasePath = "docs/projects";
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
        /// Get the file path for a project's memory file
        /// </summary>
        private static string GetProjectMemoryPath(string projectName)
        {
            var sanitized = SanitizeProjectName(projectName);
            return Path.Combine(ProjectsBasePath, sanitized, "memory.json");
        }

        /// <summary>
        /// Sanitize a project name for safe file system usage
        /// </summary>
        private static string SanitizeProjectName(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName)) return "default";
            var sanitized = new string(projectName.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.').ToArray());
            sanitized = sanitized.Trim('.').ToLowerInvariant();
            return string.IsNullOrEmpty(sanitized) ? "default" : sanitized;
        }

        /// <summary>
        /// Load persistent memories from disk (scans all project directories)
        /// </summary>
        private async Task LoadPersistentMemoriesAsync()
        {
            try
            {
                if (!Directory.Exists(ProjectsBasePath))
                {
                    _logger.LogDebug("[MEMORY:LOAD] No projects directory found at {Path}, creating it", ProjectsBasePath);
                    Directory.CreateDirectory(ProjectsBasePath);
                    return;
                }

                var memoryFiles = Directory.GetFiles(ProjectsBasePath, "memory.json", SearchOption.AllDirectories);

                await _fileSemaphore.WaitAsync();
                try
                {
                    _persistentMemories.Clear();
                    foreach (var file in memoryFiles)
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var memories = JsonSerializer.Deserialize<List<GaiaMemory>>(json);

                        if (memories != null)
                        {
                            foreach (var memory in memories)
                            {
                                _persistentMemories[memory.CompositeKey] = memory;
                            }

                            _logger.LogInformation(
                                "[MEMORY:LOAD] Loaded {Count} memories from {Path}",
                                memories.Count,
                                file);
                        }
                    }

                    _logger.LogInformation(
                        "[MEMORY:LOAD] Total: {Count} persistent memories loaded from {FileCount} project(s)",
                        _persistentMemories.Count,
                        memoryFiles.Length);
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
        /// Save persistent memories to disk grouped by project (called only by write processor)
        /// </summary>
        private async Task SavePersistentMemoriesToDiskAsync()
        {
            try
            {
                await _fileSemaphore.WaitAsync();
                try
                {
                    // Group memories by project and save each to its own file
                    var byProject = _persistentMemories.Values
                        .GroupBy(m => SanitizeProjectName(m.ProjectName))
                        .ToList();

                    foreach (var group in byProject)
                    {
                        var path = GetProjectMemoryPath(group.Key);
                        var directory = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        var memories = group.OrderBy(m => m.Category).ThenBy(m => m.Key).ToList();
                        var json = JsonSerializer.Serialize(memories, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        await File.WriteAllTextAsync(path, json);

                        _logger.LogDebug(
                            "[MEMORY:SAVE] Saved {Count} memories for project '{Project}' to {Path}",
                            memories.Count,
                            group.Key,
                            path);
                    }
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
            [Description("The project name this memory belongs to (e.g., 'my-web-app', 'gaia-toolkit'). Used to scope memories per project.")] string projectName = "default",
            [Description("Category grouping for the memory (e.g., issue, workaround, config, pattern, decision)")] string category = "",
            [Description("Unique key identifier within the category")] string key = "",
            [Description("The actual content/value to remember")] string content = "",
            [Description("Duration of memory persistence: SessionLength (lost on restart) or ProjectWide (permanently stored)")] MemoryDuration duration = MemoryDuration.SessionLength)
        {
            _logger.LogDebug(
                "[MEMORY:STORE] Starting | Project={Project} | Category={Category} | Key={Key} | Duration={Duration} | ValueLength={ValueLength}",
                projectName,
                category,
                key,
                duration,
                content?.Length ?? 0);

            try
            {
                var memory = new GaiaMemory
                {
                    ProjectName = SanitizeProjectName(projectName),
                    Category = category,
                    Key = key,
                    Value = content ?? string.Empty,
                    Duration = duration,
                    Updated = DateTime.UtcNow
                };
                var compositeKey = memory.CompositeKey;

                // Select appropriate storage based on duration
                var targetStorage = duration == MemoryDuration.ProjectWide
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
                if (duration == MemoryDuration.SessionLength)
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
                        "[MEMORY:UPDATED] Project={Project} | Category={Category} | Key={Key} | Duration={Duration} | ValueLength={ValueLength} | Session={SessionCount} | Persistent={PersistentCount}",
                        projectName,
                        category,
                        key,
                        duration,
                        content?.Length ?? 0,
                        _sessionMemories.Count,
                        _persistentMemories.Count);
                }
                else
                {
                    _logger.LogInformation(
                        "[MEMORY:STORED] Project={Project} | Category={Category} | Key={Key} | Duration={Duration} | ValueLength={ValueLength} | Session={SessionCount} | Persistent={PersistentCount}",
                        projectName,
                        category,
                        key,
                        duration,
                        content?.Length ?? 0,
                        _sessionMemories.Count,
                        _persistentMemories.Count);
                }

                return Task.FromResult(new RememberResponse
                {
                    Success = true,
                    Message = $"Memory {(isUpdate ? "updated" : "stored")} ({duration}): {category}/{key}",
                    WasUpdate = isUpdate,
                    Memory = memory
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MEMORY:STORE] Failed | Category={Category} | Key={Key} | Error={ErrorMessage}",
                    category,
                    key,
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
            [Description("The project name to search memories within (e.g., 'my-web-app', 'gaia-toolkit'). Memories are scoped per project.")] string projectName = "default",
            [Description("Query to search for in memories (supports fuzzy search across category, key, and value)")] string query = "",
            [Description("Maximum number of results to return (default: 20)")] int maxResults = 20)
        {
            var totalMemories = _sessionMemories.Count + _persistentMemories.Count;

            _logger.LogDebug(
                "[MEMORY:RECALL] Starting | Project={Project} | Query={Query} | MaxResults={MaxResults} | Session={SessionCount} | Persistent={PersistentCount}",
                projectName,
                query,
                maxResults,
                _sessionMemories.Count,
                _persistentMemories.Count);

            try
            {
                if (totalMemories == 0)
                {
                    _logger.LogInformation("[MEMORY:RECALL] No memories in store | Project={Project} | Query={Query}", projectName, query);

                    return Task.FromResult(new RecallResponse
                    {
                        Count = 0,
                        TotalMatches = 0,
                        Query = query,
                        Message = "No memories found. Use remember() to store memories first.",
                        Results = new List<MemorySearchResult>()
                    });
                }

                var scoredResults = new List<(GaiaMemory memory, double score)>();
                var queryLower = query.ToLowerInvariant();
                var queryWords = queryLower.Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);

                // Aggregate all memories from both storages, filtered by project
                var sanitizedProject = SanitizeProjectName(projectName);
                var allMemories = _sessionMemories.Values
                    .Where(m => SanitizeProjectName(m.ProjectName) == sanitizedProject)
                    .Concat(_persistentMemories.Values
                        .Where(m => SanitizeProjectName(m.ProjectName) == sanitizedProject));

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
                        query,
                        totalMemories);

                    return Task.FromResult(new RecallResponse
                    {
                        Count = 0,
                        TotalMatches = 0,
                        Query = query,
                        Message = $"No memories found matching '{query}'",
                        Results = new List<MemorySearchResult>()
                    });
                }

                var topResults = scoredResults
                    .OrderByDescending(r => r.score)
                    .Take(maxResults)
                    .Select(r => new MemorySearchResult
                    {
                        Memory = r.memory,
                        Relevance = Math.Round(r.score, 1)
                    })
                    .ToList();

                _logger.LogInformation(
                    "[MEMORY:RECALL] Completed | Query={Query} | TotalMatches={TotalMatches} | Returned={ReturnedCount} | TopScore={TopScore}",
                    query,
                    scoredResults.Count,
                    topResults.Count,
                    topResults.FirstOrDefault()?.Relevance ?? 0);

                return Task.FromResult(new RecallResponse
                {
                    Count = topResults.Count,
                    TotalMatches = scoredResults.Count,
                    Query = query,
                    Results = topResults
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MEMORY:RECALL] Failed | Query={Query} | Error={ErrorMessage}",
                    query,
                    ex.Message);

                return Task.FromResult(new RecallResponse
                {
                    Count = 0,
                    TotalMatches = 0,
                    Query = query,
                    Message = $"Error recalling memories: {ex.Message}",
                    Results = new List<MemorySearchResult>()
                });
            }
        }

        /// <summary>
        /// Clear all memories (useful for starting fresh)
        /// </summary>
        [McpServerTool]
        [Description("Clear all memories from memory (useful for starting fresh). Optionally scoped to a specific project.")]
        public async Task<ClearResponse> clear_memories(
            [Description("Optional project name to clear memories for. If not provided, clears all memories.")] string? projectName = null)
        {
            int sessionCount;
            int persistentCount;

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                var sanitizedProject = SanitizeProjectName(projectName);

                var sessionKeysToRemove = _sessionMemories
                    .Where(kvp => SanitizeProjectName(kvp.Value.ProjectName) == sanitizedProject)
                    .Select(kvp => kvp.Key).ToList();
                sessionCount = sessionKeysToRemove.Count;
                foreach (var key in sessionKeysToRemove)
                    _sessionMemories.TryRemove(key, out _);

                var persistentKeysToRemove = _persistentMemories
                    .Where(kvp => SanitizeProjectName(kvp.Value.ProjectName) == sanitizedProject)
                    .Select(kvp => kvp.Key).ToList();
                persistentCount = persistentKeysToRemove.Count;
                foreach (var key in persistentKeysToRemove)
                    _persistentMemories.TryRemove(key, out _);

                await SavePersistentMemoriesToDiskAsync();

                _logger.LogWarning(
                    "[MEMORY:CLEAR] Memories cleared for project '{Project}' | SessionCleared={SessionCount} | PersistentCleared={PersistentCount}",
                    sanitizedProject, sessionCount, persistentCount);
            }
            else
            {
                sessionCount = _sessionMemories.Count;
                persistentCount = _persistentMemories.Count;

                _sessionMemories.Clear();
                _persistentMemories.Clear();

                await SavePersistentMemoriesToDiskAsync();

                _logger.LogWarning(
                    "[MEMORY:CLEAR] All memories cleared | SessionCleared={SessionCount} | PersistentCleared={PersistentCount} | TotalCleared={TotalCount}",
                    sessionCount, persistentCount, sessionCount + persistentCount);
            }

            var totalCount = sessionCount + persistentCount;
            return new ClearResponse
            {
                Success = true,
                Message = string.IsNullOrWhiteSpace(projectName)
                    ? $"Cleared {totalCount} memories (Session: {sessionCount}, Persistent: {persistentCount})"
                    : $"Cleared {totalCount} memories for project '{projectName}' (Session: {sessionCount}, Persistent: {persistentCount})",
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
