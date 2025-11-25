using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ModelContextProtocol.Abstractions;
using ModelContextProtocol.Server.Attributes;
using Microsoft.Extensions.Logging;

namespace FrostAura.MCP.Gaia.Managers
{
    /// <summary>
    /// Core MCP Manager - Essential tools for task and memory management using JSONL
    /// </summary>
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
        /// Read current tasks from JSONL file
        /// </summary>
        [McpServerTool("read_tasks", "Get current tasks from JSONL file")]
        public async Task<ToolResponse> ReadTasksAsync()
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
                        if (task != null) tasks.Add(task);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Skipping malformed task line: {ex.Message}");
                    }
                }

                return new ToolResponse
                {
                    Content = JsonSerializer.Serialize(new
                    {
                        total = tasks.Count,
                        tasks = tasks
                    }, new JsonSerializerOptions { WriteIndented = true })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading tasks");
                return new ToolResponse { IsError = true, Content = $"Error reading tasks: {ex.Message}" };
            }
        }

        /// <summary>
        /// Update or add a task in JSONL file
        /// </summary>
        [McpServerTool("update_task", "Update or add a task")]
        public async Task<ToolResponse> UpdateTaskAsync(
            [McpServerToolParameter("Task ID (for updates) or title (for new tasks)")] string taskId,
            [McpServerToolParameter("Task description")] string description,
            [McpServerToolParameter("Status: pending, in_progress, completed, blocked")] string status,
            [McpServerToolParameter("Assigned agent (optional)", Required = false)] string assignedTo = null)
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
                    // Add as new task
                    lines.Add(JsonSerializer.Serialize(task));
                }

                await File.WriteAllLinesAsync(_tasksPath, lines.Where(l => !string.IsNullOrWhiteSpace(l)));

                return new ToolResponse
                {
                    Content = $"Task '{taskId}' {(updated ? "updated" : "added")} with status: {status}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return new ToolResponse { IsError = true, Content = $"Error updating task: {ex.Message}" };
            }
        }

        /// <summary>
        /// Store important decision or context for future reference
        /// </summary>
        [McpServerTool("remember", "Store important decisions/context in JSONL")]
        public async Task<ToolResponse> RememberAsync(
            [McpServerToolParameter("Category: system, architecture, design, decision, learning")] string category,
            [McpServerToolParameter("Key identifier for this memory")] string key,
            [McpServerToolParameter("The content to remember")] string value)
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

                return new ToolResponse
                {
                    Content = $"Memory stored: {category}/{key}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing memory");
                return new ToolResponse { IsError = true, Content = $"Error storing memory: {ex.Message}" };
            }
        }

        /// <summary>
        /// Search and recall previous decisions/context
        /// </summary>
        [McpServerTool("recall", "Search previous decisions/context from JSONL")]
        public async Task<ToolResponse> RecallAsync(
            [McpServerToolParameter("Search query to find relevant memories")] string query)
        {
            try
            {
                if (!File.Exists(_memoryPath))
                {
                    return new ToolResponse { Content = "No memories found. Memory file doesn't exist yet." };
                }

                var lines = await File.ReadAllLinesAsync(_memoryPath);
                var results = new List<Dictionary<string, object>>();

                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    try
                    {
                        var memory = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                        if (memory != null)
                        {
                            var content = JsonSerializer.Serialize(memory);
                            if (content.ToLower().Contains(query.ToLower()))
                            {
                                results.Add(memory);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Skipping malformed memory line: {ex.Message}");
                    }
                }

                if (results.Count == 0)
                {
                    return new ToolResponse { Content = $"No memories found matching '{query}'" };
                }

                return new ToolResponse
                {
                    Content = JsonSerializer.Serialize(new
                    {
                        count = results.Count,
                        query = query,
                        memories = results
                    }, new JsonSerializerOptions { WriteIndented = true })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalling memories");
                return new ToolResponse { IsError = true, Content = $"Error recalling memories: {ex.Message}" };
            }
        }

        private async Task InitializeTasksFileAsync()
        {
            var initialTasks = new[]
            {
                new Dictionary<string, object>
                {
                    ["id"] = "init",
                    ["description"] = "System initialized with simplified MCP tools",
                    ["status"] = "completed",
                    ["updated"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var lines = initialTasks.Select(t => JsonSerializer.Serialize(t));
            await File.WriteAllLinesAsync(_tasksPath, lines);
        }
    }
}