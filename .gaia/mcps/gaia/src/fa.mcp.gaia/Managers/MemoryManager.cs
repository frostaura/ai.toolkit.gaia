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
    /// </summary>
    [McpServerToolType]
    public class MemoryManager
    {
        private readonly ILogger<MemoryManager> _logger;
        private const string PersistentMemoryPath = ".gaia/memory.json";
        private static readonly SemaphoreSlim _fileSemaphore = new(1, 1);

        // Thread-safe in-memory storage - session-level memories (lost on service restart)
        private static readonly ConcurrentDictionary<string, GaiaMemory> _sessionMemories = new();

        // Thread-safe in-memory storage - project-wide memories (persisted to disk)
        private static readonly ConcurrentDictionary<string, GaiaMemory> _persistentMemories = new();

        public MemoryManager(ILogger<MemoryManager> logger)
        {
            _logger = logger;

            // Load persistent memories from disk
            LoadPersistentMemoriesAsync().Wait();

            _logger.LogInformation(
                "[STARTUP] MemoryManager initialized | SessionMemories={SessionCount} | PersistentMemories={PersistentCount}",
                _sessionMemories.Count,
                _persistentMemories.Count);
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
        /// Save persistent memories to disk
        /// </summary>
        private async Task SavePersistentMemoriesAsync()
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
        public async Task<RememberResponse> remember(
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

                targetStorage[compositeKey] = memory;

                // Save to disk if persistent
                if (request.Duration == MemoryDuration.ProjectWide)
                {
                    await SavePersistentMemoriesAsync();
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

                return new RememberResponse
                {
                    Success = true,
                    Message = $"Memory {(isUpdate ? "updated" : "stored")} ({request.Duration}): {request.Category}/{request.Key}",
                    WasUpdate = isUpdate,
                    Memory = memory
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MEMORY:STORE] Failed | Category={Category} | Key={Key} | Error={ErrorMessage}",
                    request.Category,
                    request.Key,
                    ex.Message);

                return new RememberResponse
                {
                    Success = false,
                    Message = $"Error storing memory: {ex.Message}",
                    WasUpdate = false,
                    Memory = null
                };
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

            // Clear the persistent file
            await SavePersistentMemoriesAsync();

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
    }
}
