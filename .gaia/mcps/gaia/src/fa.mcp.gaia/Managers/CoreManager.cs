using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers
{
    /// <summary>
    /// Core MCP Manager - Essential tools for task and memory management using JSONL
    /// </summary>
    [McpServerToolType]
    public class CoreManager
    {
        // Obfuscated state files in hidden directory to prevent direct AI agent access
        // These files should ONLY be accessed via MCP tools, never directly edited
        private readonly string _tasksPath = ".gaia/tasks.jsonl";
        private readonly string _memoryPath = ".gaia/memory.jsonl";
        private readonly ILogger<CoreManager> _logger;

        // Semaphores for thread-safe file access (prevents concurrent write corruption)
        private static readonly SemaphoreSlim _tasksLock = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim _memoryLock = new SemaphoreSlim(1, 1);

        public CoreManager(ILogger<CoreManager> logger)
        {
            _logger = logger;

            // Initialize files if they don't exist or are corrupt
            InitializeFilesIfNeeded();
        }

        /// <summary>
        /// Safely write lines to a file with atomic write pattern to prevent corruption
        /// </summary>
        private async Task SafeWriteAllLinesAsync(string path, IEnumerable<string> lines)
        {
            var tempPath = path + ".tmp";
            var content = string.Join(Environment.NewLine, lines);

            // Write to temp file first
            await File.WriteAllTextAsync(tempPath, content, Encoding.UTF8);

            // Atomic rename (replace) - this is atomic on most file systems
            File.Move(tempPath, path, overwrite: true);
        }

        /// <summary>
        /// Safely read all lines from a file
        /// </summary>
        private async Task<List<string>> SafeReadAllLinesAsync(string path)
        {
            if (!File.Exists(path))
                return new List<string>();

            var content = await File.ReadAllTextAsync(path, Encoding.UTF8);
            return content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void InitializeFilesIfNeeded()
        {
            // Check and initialize tasks file
            if (!IsFileValid(_tasksPath))
            {
                _logger.LogWarning("Task state missing or corrupt, initializing fresh state");
                File.WriteAllText(_tasksPath, string.Empty);
            }

            // Check and initialize memory file
            if (!IsFileValid(_memoryPath))
            {
                _logger.LogWarning("Memory state missing or corrupt, initializing fresh state");
                File.WriteAllText(_memoryPath, string.Empty);
            }
        }

        private bool IsFileValid(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                // Validate JSONL format by attempting to parse each line
                var lines = File.ReadAllLines(path);
                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                }
                return true;
            }
            catch
            {
                // File exists but is corrupt - back it up before recreating
                var backupPath = $"{path}.corrupt.{DateTime.UtcNow:yyyyMMddHHmmss}";
                try
                {
                    File.Move(path, backupPath);
                    _logger.LogWarning("Corrupt state file backed up");
                }
                catch { }
                return false;
            }
        }

        /// <summary>
        /// Get current tasks from JSONL file with optional filtering
        /// </summary>
        [McpServerTool]
        [Description("Get current tasks from JSONL file with optional filtering")]
        public async Task<string> read_tasks(
            [Description("Hide completed tasks (default: false)")] bool hideCompleted = false)
        {
            await _tasksLock.WaitAsync();
            try
            {
                var lines = await SafeReadAllLinesAsync(_tasksPath);
                var tasks = new List<object>();

                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    try
                    {
                        var task = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                        if (task != null)
                        {
                            // Filter out completed tasks if requested
                            if (hideCompleted && task.ContainsKey("status"))
                            {
                                var status = task["status"]?.ToString()?.ToLower();
                                if (status == "completed" || status == "done") continue;
                            }
                            tasks.Add(task);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Skipping malformed task line: {ex.Message}");
                    }
                }

                var summary = hideCompleted
                    ? $"{tasks.Count} active/pending tasks"
                    : $"{tasks.Count} total tasks";

                return JsonSerializer.Serialize(new
                {
                    summary = summary,
                    filter = hideCompleted ? "active only" : "all tasks",
                    count = tasks.Count,
                    tasks = tasks
                }, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading tasks");
                return $"Error reading tasks: {ex.Message}";
            }
            finally
            {
                _tasksLock.Release();
            }
        }

        /// <summary>
        /// Update or add a task
        /// </summary>
        [McpServerTool]
        [Description("Update or add a task")]
        public async Task<string> update_task(
            [Description("ID of the task")] string taskId,
            [Description("Description of the task")] string description,
            [Description("Status of the task")] string status,
            [Description("Who the task is assigned to")] string? assignedTo = null)
        {
            await _tasksLock.WaitAsync();
            try
            {
                var task = new Dictionary<string, object>
                {
                    ["id"] = taskId?.Replace(" ", "_").ToLower() ?? Guid.NewGuid().ToString(),
                    ["description"] = description,
                    ["status"] = status,
                    ["updated"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                if (!string.IsNullOrEmpty(assignedTo))
                {
                    task["assigned_to"] = assignedTo;
                }

                // Check if task exists and update, or append new
                var lines = await SafeReadAllLinesAsync(_tasksPath);

                var updated = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    try
                    {
                        var existingTask = JsonSerializer.Deserialize<Dictionary<string, object>>(lines[i]);
                        if (existingTask != null && existingTask.ContainsKey("id") &&
                            existingTask["id"].ToString() == task["id"].ToString())
                        {
                            lines[i] = JsonSerializer.Serialize(task);
                            updated = true;
                            break;
                        }
                    }
                    catch { }
                }

                if (!updated)
                {
                    lines.Add(JsonSerializer.Serialize(task));
                }

                await SafeWriteAllLinesAsync(_tasksPath, lines.Where(l => !string.IsNullOrWhiteSpace(l)));

                return $"Task '{taskId}' {(updated ? "updated" : "added")} with status: {status}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return $"Error updating task: {ex.Message}";
            }
            finally
            {
                _tasksLock.Release();
            }
        }

        /// <summary>
        /// Store important decisions/context for later recalling (upserts by category+key)
        /// </summary>
        [McpServerTool]
        [Description("Store important decisions/context for later recalling. Upserts by category+key to prevent duplicates.")]
        public async Task<string> remember(
            [Description("Category of the memory")] string category,
            [Description("Key identifier for the memory")] string key,
            [Description("Value/content to remember")] string value)
        {
            await _memoryLock.WaitAsync();
            try
            {
                var memory = new Dictionary<string, object>
                {
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["category"] = category,
                    ["key"] = key,
                    ["value"] = value
                };

                // Read existing memories and upsert by category+key
                var lines = await SafeReadAllLinesAsync(_memoryPath);

                var updated = false;
                var compositeKey = $"{category}/{key}".ToLower();

                for (int i = 0; i < lines.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    try
                    {
                        var existingMemory = JsonSerializer.Deserialize<Dictionary<string, object>>(lines[i]);
                        if (existingMemory != null &&
                            existingMemory.ContainsKey("category") &&
                            existingMemory.ContainsKey("key"))
                        {
                            var existingKey = $"{existingMemory["category"]}/{existingMemory["key"]}".ToLower();
                            if (existingKey == compositeKey)
                            {
                                lines[i] = JsonSerializer.Serialize(memory);
                                updated = true;
                                break;
                            }
                        }
                    }
                    catch { }
                }

                if (!updated)
                {
                    lines.Add(JsonSerializer.Serialize(memory));
                }

                await SafeWriteAllLinesAsync(_memoryPath, lines.Where(l => !string.IsNullOrWhiteSpace(l)));

                return $"Memory {(updated ? "updated" : "stored")}: {category}/{key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing memory");
                return $"Error storing memory: {ex.Message}";
            }
            finally
            {
                _memoryLock.Release();
            }
        }

        /// <summary>
        /// Search previous decisions/context with fuzzy matching
        /// </summary>
        [McpServerTool]
        [Description("Search previous decisions/context with fuzzy matching")]
        public async Task<string> recall(
            [Description("Query to search for in memories (supports fuzzy search)")] string query,
            [Description("Maximum number of results to return (default: 20)")] int maxResults = 20)
        {
            await _memoryLock.WaitAsync();
            try
            {
                var lines = await SafeReadAllLinesAsync(_memoryPath);

                if (lines.Count == 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        count = 0,
                        query = query,
                        message = "No memories found. Use mcp__gaia__remember to store memories first.",
                        memories = new List<object>()
                    }, new JsonSerializerOptions { WriteIndented = true });
                }

                var scoredResults = new List<(Dictionary<string, object> memory, double score)>();

                // Split query into words for fuzzy matching
                var queryWords = query.ToLower().Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    try
                    {
                        var memory = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                        if (memory != null)
                        {
                            var content = JsonSerializer.Serialize(memory).ToLower();
                            double score = 0;

                            // Exact match (highest score)
                            if (content.Contains(query.ToLower()))
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
                                    var index = content.IndexOf(word);
                                    if (index >= 0)
                                    {
                                        wordsFound++;
                                        // Earlier matches get higher scores
                                        totalPositionScore += Math.Max(0, 100 - (index / 10));
                                    }
                                }

                                if (wordsFound > 0)
                                {
                                    // Score based on percentage of words found and their positions
                                    score = (wordsFound * 60.0 / queryWords.Length) + (totalPositionScore / queryWords.Length * 0.4);

                                    // Bonus for category/key matches
                                    if (memory.ContainsKey("category") && memory["category"]?.ToString()?.ToLower().Contains(query.ToLower()) == true)
                                        score += 20;
                                    if (memory.ContainsKey("key") && memory["key"]?.ToString()?.ToLower().Contains(query.ToLower()) == true)
                                        score += 15;
                                }
                            }

                            if (score > 0)
                            {
                                scoredResults.Add((memory, score));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Skipping malformed memory line: {ex.Message}");
                    }
                }

                if (scoredResults.Count == 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        count = 0,
                        query = query,
                        message = $"No memories found matching '{query}'",
                        memories = new List<object>()
                    }, new JsonSerializerOptions { WriteIndented = true });
                }

                // Sort by score descending and take top N results
                var topResults = scoredResults
                    .OrderByDescending(r => r.score)
                    .Take(maxResults)
                    .Select(r => new
                    {
                        memory = r.memory,
                        relevance = Math.Round(r.score, 1)
                    })
                    .ToList();

                return JsonSerializer.Serialize(new
                {
                    count = topResults.Count,
                    totalMatches = scoredResults.Count,
                    query = query,
                    searchMode = "fuzzy",
                    results = topResults
                }, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalling memories");
                return $"Error recalling memories: {ex.Message}";
            }
            finally
            {
                _memoryLock.Release();
            }
        }
    }
}
