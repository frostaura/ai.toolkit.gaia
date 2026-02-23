using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers
{
    /// <summary>
    /// Task Manager - Essential tools for in-memory task management
    /// Data is ephemeral and will be lost when the service stops
    /// Uses per-project isolation with TTL to prevent cross-user interference
    /// </summary>
    [McpServerToolType]
    public class TaskManager : IDisposable
    {
        private readonly ILogger<TaskManager> _logger;

        // Thread-safe per-project storage - each project gets isolated task storage
        // This prevents remote MCP users from overriding each other's tasks
        private static readonly ConcurrentDictionary<string, ProjectTaskStore> _projectStores = new();

        // TTL for project stores (default: 24 hours of inactivity)
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(24);

        // Background cleanup processor
        private static Timer? _cleanupTimer;
        private static readonly object _cleanupLock = new();
        private static bool _cleanupStarted = false;
        private static bool _disposed = false;

        // Cleanup interval (check every 5 minutes)
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);

        public TaskManager(ILogger<TaskManager> logger)
        {
            _logger = logger;

            // Start the background cleanup processor (only once across all instances)
            StartCleanupProcessor();

            var totalTasks = _projectStores.Values.Sum(s => s.Tasks.Count);
            _logger.LogInformation(
                "[STARTUP] TaskManager initialized | Storage=InMemory | Projects={ProjectCount} | TotalTasks={TaskCount} | TTL={TTL}",
                _projectStores.Count,
                totalTasks,
                DefaultTtl);
        }

        /// <summary>
        /// Start the background cleanup timer for TTL expiration
        /// </summary>
        private void StartCleanupProcessor()
        {
            lock (_cleanupLock)
            {
                if (_cleanupStarted || _disposed) return;

                _cleanupStarted = true;
                _cleanupTimer = new Timer(
                    CleanupExpiredStores,
                    null,
                    CleanupInterval,
                    CleanupInterval);

                _logger.LogInformation(
                    "[TASK:CLEANUP] Background cleanup processor started | Interval={Interval} | TTL={TTL}",
                    CleanupInterval,
                    DefaultTtl);
            }
        }

        /// <summary>
        /// Cleanup callback - removes expired project stores
        /// </summary>
        private void CleanupExpiredStores(object? state)
        {
            try
            {
                var expiredProjects = _projectStores
                    .Where(kvp => kvp.Value.IsExpired(DefaultTtl))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var projectName in expiredProjects)
                {
                    if (_projectStores.TryRemove(projectName, out var store))
                    {
                        _logger.LogInformation(
                            "[TASK:CLEANUP] Expired project store removed | Project={Project} | TaskCount={TaskCount} | LastAccessed={LastAccessed}",
                            projectName,
                            store.Tasks.Count,
                            store.LastAccessed);
                    }
                }

                if (expiredProjects.Count > 0)
                {
                    _logger.LogInformation(
                        "[TASK:CLEANUP] Cleanup completed | RemovedProjects={RemovedCount} | RemainingProjects={RemainingCount}",
                        expiredProjects.Count,
                        _projectStores.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TASK:CLEANUP] Cleanup failed | Error={ErrorMessage}", ex.Message);
            }
        }

        /// <summary>
        /// Get or create a project task store (thread-safe)
        /// </summary>
        private ProjectTaskStore GetOrCreateProjectStore(string sanitizedProject)
        {
            var store = _projectStores.GetOrAdd(sanitizedProject, name => new ProjectTaskStore(name));
            store.Touch(); // Update last accessed time
            return store;
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
                var store = GetOrCreateProjectStore(sanitizedProject);
                var tasks = store.Tasks.Values.AsEnumerable();

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
                    "[TASK:READ] Completed | Project={Project} | Filter={Filter} | TotalInStore={TotalTasks} | Returned={ReturnedCount}",
                    sanitizedProject,
                    response.Filter,
                    store.Tasks.Count,
                    taskList.Count);

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TASK:READ] Failed | Project={Project} | Filter={Filter} | Error={ErrorMessage}",
                    sanitizedProject,
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
            [Description("The project name this task belongs to (e.g., 'my-web-app', 'gaia-toolkit'). Tasks are scoped per project.")] string projectName = "default",
            [Description("Unique identifier for the task (e.g., E-1/S-1/F-1/T-1 for hierarchical WBS)")] string taskId = "",
            [Description("Detailed description of what the task involves")] string description = "",
            [Description("Current status of the task: Pending, InProgress, Completed, Blocked, or Cancelled")] GaiaTaskStatus status = GaiaTaskStatus.Pending,
            [Description("The agent or person assigned to complete this task (optional)")] string? assignedTo = null)
        {
            _logger.LogDebug(
                "[TASK:UPDATE] Starting | Project={Project} | TaskId={TaskId} | Status={Status} | AssignedTo={AssignedTo}",
                projectName,
                taskId,
                status,
                assignedTo ?? "(unassigned)");

            try
            {
                var sanitizedProject = SanitizeProjectName(projectName);
                var normalizedId = taskId?.Replace(" ", "_").ToLowerInvariant() ?? Guid.NewGuid().ToString();
                var store = GetOrCreateProjectStore(sanitizedProject);
                var isUpdate = store.Tasks.TryGetValue(normalizedId, out var existingTask);
                var previousStatus = existingTask?.Status;

                var task = new GaiaTask
                {
                    ProjectName = sanitizedProject,
                    Id = normalizedId,
                    Description = description,
                    Status = status,
                    AssignedTo = string.IsNullOrWhiteSpace(assignedTo) ? null : assignedTo,
                    Created = existingTask?.Created ?? DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                store.Tasks[normalizedId] = task;

                if (isUpdate)
                {
                    _logger.LogInformation(
                        "[TASK:UPDATED] Project={Project} | TaskId={TaskId} | PreviousStatus={PreviousStatus} | NewStatus={NewStatus} | AssignedTo={AssignedTo} | ProjectTasks={ProjectTasks}",
                        sanitizedProject,
                        normalizedId,
                        previousStatus,
                        status,
                        task.AssignedTo ?? "(unassigned)",
                        store.Tasks.Count);
                }
                else
                {
                    _logger.LogInformation(
                        "[TASK:CREATED] Project={Project} | TaskId={TaskId} | Status={Status} | AssignedTo={AssignedTo} | ProjectTasks={ProjectTasks}",
                        sanitizedProject,
                        normalizedId,
                        status,
                        task.AssignedTo ?? "(unassigned)",
                        store.Tasks.Count);
                }

                return Task.FromResult(new UpdateTaskResponse
                {
                    Success = true,
                    Message = $"Task '{taskId}' {(isUpdate ? "updated" : "added")} with status: {status}",
                    Task = task
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[TASK:UPDATE] Failed | Project={Project} | TaskId={TaskId} | Error={ErrorMessage}",
                    projectName,
                    taskId,
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

                if (_projectStores.TryGetValue(sanitizedProject, out var store))
                {
                    count = store.Tasks.Count;
                    store.Tasks.Clear();
                    store.Touch(); // Update last accessed time

                    _logger.LogWarning(
                        "[TASK:CLEAR] Tasks cleared for project '{Project}' | ClearedCount={ClearedCount} | RemainingProjects={RemainingProjects}",
                        sanitizedProject,
                        count,
                        _projectStores.Count);
                }
                else
                {
                    count = 0;
                    _logger.LogInformation(
                        "[TASK:CLEAR] No tasks found for project '{Project}'",
                        sanitizedProject);
                }
            }
            else
            {
                count = _projectStores.Values.Sum(s => s.Tasks.Count);
                _projectStores.Clear();

                _logger.LogWarning(
                    "[TASK:CLEAR] All tasks cleared | ClearedCount={ClearedCount} | ClearedProjects={ClearedProjects}",
                    count,
                    _projectStores.Count);
            }

            return Task.FromResult(new ClearResponse
            {
                Success = true,
                Message = $"Cleared {count} tasks from memory",
                ClearedCount = count
            });
        }

        /// <summary>
        /// Dispose of managed resources
        /// </summary>
        public void Dispose()
        {
            lock (_cleanupLock)
            {
                if (_disposed) return;
                _disposed = true;
                _cleanupStarted = false;

                _cleanupTimer?.Dispose();
                _cleanupTimer = null;

                _logger.LogInformation("[TASK:SHUTDOWN] TaskManager disposed | Projects={ProjectCount} | TotalTasks={TaskCount}",
                    _projectStores.Count,
                    _projectStores.Values.Sum(s => s.Tasks.Count));
            }
        }
    }
}
