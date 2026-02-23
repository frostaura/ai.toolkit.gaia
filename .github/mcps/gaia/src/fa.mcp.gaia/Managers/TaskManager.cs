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
    /// Task Manager - Essential tools for in-memory task management
    /// Data is ephemeral and will be lost when the service stops
    /// </summary>
    [McpServerToolType]
    public class TaskManager
    {
        private readonly ILogger<TaskManager> _logger;

        // Thread-safe in-memory storage - data is ephemeral (lost on service restart)
        private static readonly ConcurrentDictionary<string, GaiaTask> _tasks = new();

        public TaskManager(ILogger<TaskManager> logger)
        {
            _logger = logger;
            _logger.LogInformation(
                "[STARTUP] TaskManager initialized | Storage=InMemory | TaskCount={TaskCount}",
                _tasks.Count);
        }

        /// <summary>
        /// Get current tasks with optional filtering
        /// </summary>
        [McpServerTool]
        [Description("Get current tasks from in-memory storage with optional filtering")]
        public Task<ReadTasksResponse> read_tasks(
            [Description("The project name to filter tasks by (e.g., 'my-web-app', 'gaia-toolkit')")] string projectName = "default",
            [Description("Hide completed and cancelled tasks (default: false)")] bool hideCompleted = false)
        {
            var sanitizedProject = SanitizeProjectName(projectName);
            _logger.LogDebug("[TASK:READ] Starting | Project={Project} | Filter={Filter}", sanitizedProject, hideCompleted ? "active only" : "all");

            try
            {
                var tasks = _tasks.Values.Where(t => SanitizeProjectName(t.ProjectName) == sanitizedProject).AsEnumerable();

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
                "[TASK:UPDATE] Starting | Project={Project} | TaskId={TaskId} | Status={Status} | AssignedTo={AssignedTo}",
                request.ProjectName,
                request.TaskId,
                request.Status,
                request.AssignedTo ?? "(unassigned)");

            try
            {
                var sanitizedProject = SanitizeProjectName(request.ProjectName);
                var normalizedId = $"{sanitizedProject}/{request.TaskId?.Replace(" ", "_").ToLowerInvariant() ?? Guid.NewGuid().ToString()}";
                var isUpdate = _tasks.TryGetValue(normalizedId, out var existingTask);
                var previousStatus = existingTask?.Status;

                var task = new GaiaTask
                {
                    ProjectName = sanitizedProject,
                    Id = request.TaskId?.Replace(" ", "_").ToLowerInvariant() ?? Guid.NewGuid().ToString(),
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
        /// Sanitize a project name for safe usage
        /// </summary>
        private static string SanitizeProjectName(string? projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName)) return "default";
            var sanitized = new string(projectName.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.').ToArray());
            sanitized = sanitized.Trim('.').ToLowerInvariant();
            return string.IsNullOrEmpty(sanitized) ? "default" : sanitized;
        }

        /// <summary>
        /// Clear all tasks (useful for starting fresh)
        /// </summary>
        [McpServerTool]
        [Description("Clear all tasks from memory (useful for starting fresh)")]
        public Task<ClearResponse> clear_tasks(
            [Description("Optional project name to clear tasks for. If not provided, clears all tasks.")] string? projectName = null)
        {
            int count;

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                var sanitizedProject = SanitizeProjectName(projectName);
                var keysToRemove = _tasks.Where(kvp => SanitizeProjectName(kvp.Value.ProjectName) == sanitizedProject)
                    .Select(kvp => kvp.Key).ToList();
                count = keysToRemove.Count;
                foreach (var key in keysToRemove)
                {
                    _tasks.TryRemove(key, out _);
                }

                _logger.LogWarning(
                    "[TASK:CLEAR] Tasks cleared for project '{Project}' | ClearedCount={ClearedCount} | RemainingTasks={RemainingTasks}",
                    sanitizedProject,
                    count,
                    _tasks.Count);
            }
            else
            {
                count = _tasks.Count;
                _tasks.Clear();

                _logger.LogWarning(
                    "[TASK:CLEAR] All tasks cleared | ClearedCount={ClearedCount} | RemainingTasks={RemainingTasks}",
                    count,
                    _tasks.Count);
            }

            return Task.FromResult(new ClearResponse
            {
                Success = true,
                Message = $"Cleared {count} tasks from memory",
                ClearedCount = count
            });
        }
    }
}
