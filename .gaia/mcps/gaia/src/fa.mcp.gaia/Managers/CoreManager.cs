using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly string _tasksPath = ".gaia/tasks.jsonl";
        private readonly string _memoryPath = ".gaia/memory.jsonl";
        private readonly ILogger<CoreManager> _logger;

        public CoreManager(ILogger<CoreManager> logger)
        {
            _logger = logger;

            // Ensure .gaia directory exists
            Directory.CreateDirectory(".gaia");
        }

        /// <summary>
        /// Get current tasks from JSONL file with optional filtering
        /// </summary>
        [McpServerTool]
        [Description("Get current tasks from JSONL file with optional filtering")]
        public async Task<string> read_tasks(
            [Description("Hide completed tasks (default: false)")] bool hideCompleted = false)
        {
            try
            {
                if (!File.Exists(_tasksPath))
                {
                    await InitializeTasksFileAsync();
                }

                var lines = await File.ReadAllLinesAsync(_tasksPath);
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
                var lines = File.Exists(_tasksPath)
                    ? (await File.ReadAllLinesAsync(_tasksPath)).ToList()
                    : new List<string>();

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

                await File.WriteAllLinesAsync(_tasksPath, lines.Where(l => !string.IsNullOrWhiteSpace(l)));

                return $"Task '{taskId}' {(updated ? "updated" : "added")} with status: {status}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return $"Error updating task: {ex.Message}";
            }
        }

        /// <summary>
        /// Store important decisions/context in JSONL
        /// </summary>
        [McpServerTool]
        [Description("Store important decisions/context in JSONL")]
        public async Task<string> remember(
            [Description("Category of the memory")] string category,
            [Description("Key identifier for the memory")] string key,
            [Description("Value/content to remember")] string value)
        {
            try
            {
                var memory = new Dictionary<string, object>
                {
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["category"] = category,
                    ["key"] = key,
                    ["value"] = value
                };

                var json = JsonSerializer.Serialize(memory);
                await File.AppendAllTextAsync(_memoryPath, json + Environment.NewLine);

                return $"Memory stored: {category}/{key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing memory");
                return $"Error storing memory: {ex.Message}";
            }
        }

        /// <summary>
        /// Search previous decisions/context from JSONL with fuzzy matching
        /// </summary>
        [McpServerTool]
        [Description("Search previous decisions/context from JSONL with fuzzy matching")]
        public async Task<string> recall(
            [Description("Query to search for in memories (supports fuzzy search)")] string query,
            [Description("Maximum number of results to return (default: 20)")] int maxResults = 20)
        {
            try
            {
                if (!File.Exists(_memoryPath))
                {
                    return "No memories found. Memory file doesn't exist yet.";
                }

                var lines = await File.ReadAllLinesAsync(_memoryPath);
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
        }

        private async Task InitializeTasksFileAsync()
        {
            var initialTasks = new[]
            {
                new Dictionary<string, object>
                {
                    ["id"] = "init",
                    ["description"] = "System initialized with core MCP tools",
                    ["status"] = "completed",
                    ["updated"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var lines = initialTasks.Select(t => JsonSerializer.Serialize(t));
            await File.WriteAllLinesAsync(_tasksPath, lines);
        }
    }
}