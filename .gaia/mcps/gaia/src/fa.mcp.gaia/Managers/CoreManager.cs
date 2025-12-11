using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers
{
    /// <summary>
    /// Core MCP Manager - Essential tools for in-memory task and memory management
    /// Data is ephemeral and will be lost when the service stops
    /// </summary>
    [McpServerToolType]
    public class CoreManager
    {
        private readonly ILogger<CoreManager> _logger;

        // Thread-safe in-memory storage - data is ephemeral (lost on service restart)
        private static readonly ConcurrentDictionary<string, GaiaTask> _tasks = new();
        private static readonly ConcurrentDictionary<string, GaiaMemory> _memories = new();

        public CoreManager(ILogger<CoreManager> logger)
        {
            _logger = logger;
            _logger.LogInformation(
                "[STARTUP] CoreManager initialized | Storage=InMemory | TaskCount={TaskCount} | MemoryCount={MemoryCount}",
                _tasks.Count,
                _memories.Count);
        }

        /// <summary>
        /// Get current tasks with optional filtering
        /// </summary>
        [McpServerTool]
        [Description("Get current tasks from in-memory storage with optional filtering")]
        public Task<ReadTasksResponse> read_tasks(
            [Description("Hide completed and cancelled tasks (default: false)")] bool hideCompleted = false)
        {
            _logger.LogDebug("[TASK:READ] Starting | Filter={Filter}", hideCompleted ? "active only" : "all");

            try
            {
                var tasks = _tasks.Values.AsEnumerable();

                if (hideCompleted)
                {
                    tasks = tasks.Where(t => t.Status != GaiaTaskStatus.Completed && t.Status != GaiaTaskStatus.Cancelled);
                }

                var taskList = tasks.OrderBy(t => t.Created).ToList();
                var response = new ReadTasksResponse
                {
                    Summary = hideCompleted
                        ? $"{taskList.Count} active/pending tasks"
                        : $"{taskList.Count} total tasks",
                    Filter = hideCompleted ? "active only" : "all tasks",
                    Count = taskList.Count,
                    Tasks = taskList
                };

                _logger.LogInformation(
                    "[TASK:READ] Completed | Filter={Filter} | TotalInStore={TotalTasks} | Returned={ReturnedCount}",
                    response.Filter,
                    _tasks.Count,
                    taskList.Count);

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TASK:READ] Failed | Filter={Filter} | Error={ErrorMessage}",
                    hideCompleted ? "active only" : "all",
                    ex.Message);

                return Task.FromResult(new ReadTasksResponse
                {
                    Summary = $"Error: {ex.Message}",
                    Filter = hideCompleted ? "active only" : "all tasks",
                    Count = 0,
                    Tasks = new List<GaiaTask>()
                });
            }
        }

        /// <summary>
        /// Update or add a task
        /// </summary>
        [McpServerTool]
        [Description("Update or add a task with structured data")]
        public Task<UpdateTaskResponse> update_task(
            [Description("The task to create or update")] UpdateTaskRequest request)
        {
            _logger.LogDebug(
                "[TASK:UPDATE] Starting | TaskId={TaskId} | Status={Status} | AssignedTo={AssignedTo}",
                request.TaskId,
                request.Status,
                request.AssignedTo ?? "(unassigned)");

            try
            {
                var normalizedId = request.TaskId?.Replace(" ", "_").ToLowerInvariant() ?? Guid.NewGuid().ToString();
                var isUpdate = _tasks.TryGetValue(normalizedId, out var existingTask);
                var previousStatus = existingTask?.Status;

                var task = new GaiaTask
                {
                    Id = normalizedId,
                    Description = request.Description,
                    Status = request.Status,
                    AssignedTo = string.IsNullOrWhiteSpace(request.AssignedTo) ? null : request.AssignedTo,
                    Created = existingTask?.Created ?? DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                _tasks[normalizedId] = task;

                if (isUpdate)
                {
                    _logger.LogInformation(
                        "[TASK:UPDATED] TaskId={TaskId} | PreviousStatus={PreviousStatus} | NewStatus={NewStatus} | AssignedTo={AssignedTo} | TotalTasks={TotalTasks}",
                        normalizedId,
                        previousStatus,
                        request.Status,
                        task.AssignedTo ?? "(unassigned)",
                        _tasks.Count);
                }
                else
                {
                    _logger.LogInformation(
                        "[TASK:CREATED] TaskId={TaskId} | Status={Status} | AssignedTo={AssignedTo} | TotalTasks={TotalTasks}",
                        normalizedId,
                        request.Status,
                        task.AssignedTo ?? "(unassigned)",
                        _tasks.Count);
                }

                return Task.FromResult(new UpdateTaskResponse
                {
                    Success = true,
                    Message = $"Task '{request.TaskId}' {(isUpdate ? "updated" : "added")} with status: {request.Status}",
                    Task = task
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[TASK:UPDATE] Failed | TaskId={TaskId} | Error={ErrorMessage}",
                    request.TaskId,
                    ex.Message);

                return Task.FromResult(new UpdateTaskResponse
                {
                    Success = false,
                    Message = $"Error updating task: {ex.Message}",
                    Task = null
                });
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
                "[MEMORY:STORE] Starting | Category={Category} | Key={Key} | ValueLength={ValueLength}",
                request.Category,
                request.Key,
                request.Value?.Length ?? 0);

            try
            {
                var memory = new GaiaMemory
                {
                    Category = request.Category,
                    Key = request.Key,
                    Value = request.Value ?? string.Empty,
                    Updated = DateTime.UtcNow
                };
                var compositeKey = memory.CompositeKey;
                var isUpdate = _memories.TryGetValue(compositeKey, out var existingMemory);

                if (isUpdate && existingMemory != null)
                {
                    memory.Created = existingMemory.Created;
                }
                else
                {
                    memory.Created = DateTime.UtcNow;
                }

                _memories[compositeKey] = memory;

                if (isUpdate)
                {
                    _logger.LogInformation(
                        "[MEMORY:UPDATED] Category={Category} | Key={Key} | ValueLength={ValueLength} | TotalMemories={TotalMemories}",
                        request.Category,
                        request.Key,
                        request.Value?.Length ?? 0,
                        _memories.Count);
                }
                else
                {
                    _logger.LogInformation(
                        "[MEMORY:STORED] Category={Category} | Key={Key} | ValueLength={ValueLength} | TotalMemories={TotalMemories}",
                        request.Category,
                        request.Key,
                        request.Value?.Length ?? 0,
                        _memories.Count);
                }

                return Task.FromResult(new RememberResponse
                {
                    Success = true,
                    Message = $"Memory {(isUpdate ? "updated" : "stored")}: {request.Category}/{request.Key}",
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
        /// </summary>
        [McpServerTool]
        [Description("Search previous decisions/context with fuzzy matching")]
        public Task<RecallResponse> recall(
            [Description("The search request")] RecallRequest request)
        {
            _logger.LogDebug(
                "[MEMORY:RECALL] Starting | Query={Query} | MaxResults={MaxResults} | TotalMemories={TotalMemories}",
                request.Query,
                request.MaxResults,
                _memories.Count);

            try
            {
                if (_memories.IsEmpty)
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

                foreach (var memory in _memories.Values)
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
                        _memories.Count);

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
        /// Clear all tasks (useful for starting fresh)
        /// </summary>
        [McpServerTool]
        [Description("Clear all tasks from memory (useful for starting fresh)")]
        public Task<ClearResponse> clear_tasks()
        {
            var count = _tasks.Count;
            _tasks.Clear();

            _logger.LogWarning(
                "[TASK:CLEAR] All tasks cleared | ClearedCount={ClearedCount} | RemainingTasks={RemainingTasks}",
                count,
                _tasks.Count);

            return Task.FromResult(new ClearResponse
            {
                Success = true,
                Message = $"Cleared {count} tasks from memory",
                ClearedCount = count
            });
        }

        /// <summary>
        /// Clear all memories (useful for starting fresh)
        /// </summary>
        [McpServerTool]
        [Description("Clear all memories from memory (useful for starting fresh)")]
        public Task<ClearResponse> clear_memories()
        {
            var count = _memories.Count;
            _memories.Clear();

            _logger.LogWarning(
                "[MEMORY:CLEAR] All memories cleared | ClearedCount={ClearedCount} | RemainingMemories={RemainingMemories}",
                count,
                _memories.Count);

            return Task.FromResult(new ClearResponse
            {
                Success = true,
                Message = $"Cleared {count} memories from memory",
                ClearedCount = count
            });
        }
    }
}
