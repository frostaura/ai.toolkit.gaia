using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Configuration;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for Task planning operations with integrated MCP tools
/// </summary>
[McpServerToolType]
public class TaskPlannerManager : ITaskPlannerManager
{
    private readonly ITaskPlannerRepository _repository;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the TaskPlannerManager
    /// </summary>
    /// <param name="repository">Task repository</param>
    /// <param name="configuration">Configuration to check for error handling preferences</param>
    public TaskPlannerManager(ITaskPlannerRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _jsonOptions = JsonConfiguration.GetApiOptions();
    }

    /// <summary>
    /// Creates a new project plan via MCP
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description that an AI can understand</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="creatorIdentity">A best attempt at a derived user name / context, typically from the host machine details</param>
    /// <returns>JSON string containing the created project plan</returns>
    [McpServerTool]
    [Description("Creates a new project plan for managing Tasks & TODOs. Ideal for tracking tasks and features of complex projects and plans. The response is your Task plan id, which you must use to manage your Tasks. Estimate hours are calculated automatically from child tasks.")]
    public async Task<string> NewPlanAsync(
        [Description("Name of the project")] string projectName,
        [Description("Brief description of the project")] string description,
        [Description("Concise context that will be needed for when the AI agent later uses the plan to build the solution")] string aiAgentBuildContext,
        [Description("A best attempt at a derived user name / context, typically from the host machine details")] string creatorIdentity)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(projectName))
            throw new ArgumentException("Project name cannot be null or empty.", nameof(projectName));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(aiAgentBuildContext))
            throw new ArgumentException("AI agent build context cannot be null or empty.", nameof(aiAgentBuildContext));
        if (string.IsNullOrWhiteSpace(creatorIdentity))
            throw new ArgumentException("Creator identity cannot be null or empty.", nameof(creatorIdentity));

        var plan = new ProjectPlan
        {
            Id = Guid.NewGuid().ToString(),
            Name = projectName,
            Description = description,
            AiAgentBuildContext = aiAgentBuildContext,
            CreatorIdentity = creatorIdentity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _repository.AddPlanAsync(plan);
        var json = JsonSerializer.Serialize(plan, _jsonOptions);
        return json;
    }

    /// <summary>
    /// Lists all project plans via MCP
    /// </summary>
    /// <param name="hideCompleted">Optional string to hide completed plans ("true" to hide, anything else to show all)</param>
    /// <returns>JSON string containing all project plans with their status and progress information</returns>
    [McpServerTool]
    [Description("Lists all project plans with their IDs, names, descriptions, progress metrics, and calculated status. Does not include tasks.")]
    public async Task<string> ListPlansAsync(
        [Description("Optional string to hide completed plans (\"true\" to hide completed plans, anything else shows all)")] string? hideCompleted = null)
    {
        // Parse hideCompleted parameter
        bool shouldHideCompleted = !string.IsNullOrEmpty(hideCompleted) && 
                                   hideCompleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Get all plans
        var plans = await _repository.GetAllPlansAsync();
        
        // Create plan summaries with calculated status and progress
        var planSummaries = new List<object>();
        
        foreach (var plan in plans)
        {
            // Get tasks for this plan to calculate status and progress
            var tasks = await _repository.GetTasksByPlanAsync(plan.Id);
            
            // Calculate plan status and progress metrics
            string planStatus = "Todo"; // Default status
            
            // Calculate progress statistics
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.Status == Enums.TaskStatus.Completed);
            var inProgressTasks = tasks.Count(t => t.Status == Enums.TaskStatus.InProgress);
            var pendingTasks = tasks.Count(t => t.Status == Enums.TaskStatus.Todo);
            
            if (tasks.Any())
            {
                if (completedTasks == totalTasks)
                {
                    planStatus = "Completed";
                }
                else if (inProgressTasks > 0 || completedTasks > 0)
                {
                    planStatus = "InProgress";
                }
                // else remains "Todo"
            }
            
            // Skip completed plans if hideCompleted is true
            if (shouldHideCompleted && planStatus == "Completed")
            {
                continue;
            }
            
            // Calculate estimate hours by status
            var totalTaskEstimateHours = tasks.Sum(t => t.EstimateHours);
            var completedTaskEstimateHours = tasks.Where(t => t.Status == Enums.TaskStatus.Completed).Sum(t => t.EstimateHours);
            var inProgressTaskEstimateHours = tasks.Where(t => t.Status == Enums.TaskStatus.InProgress).Sum(t => t.EstimateHours);
            var pendingTaskEstimateHours = tasks.Where(t => t.Status == Enums.TaskStatus.Todo).Sum(t => t.EstimateHours);
            
            // Calculate completion percentages
            var completionPercentage = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100, 2) : 0.0;
            var estimateCompletionPercentage = totalTaskEstimateHours > 0 ? Math.Round(completedTaskEstimateHours / totalTaskEstimateHours * 100, 2) : 0.0;
            
            planSummaries.Add(new
            {
                id = plan.Id,
                name = plan.Name,
                description = plan.Description,
                aiAgentBuildContext = plan.AiAgentBuildContext,
                creatorIdentity = plan.CreatorIdentity,
                estimateHours = plan.EstimateHours,
                status = planStatus,
                progress = new
                {
                    totalTasks,
                    completedTasks,
                    inProgressTasks,
                    pendingTasks,
                    completionPercentage,
                    totalTaskEstimateHours,
                    completedTaskEstimateHours,
                    inProgressTaskEstimateHours,
                    pendingTaskEstimateHours,
                    estimateCompletionPercentage
                },
                createdAt = plan.CreatedAt,
                updatedAt = plan.UpdatedAt
            });
        }
        
        var json = JsonSerializer.Serialize(planSummaries, _jsonOptions);
        return json;
    }

    /// <summary>
    /// Gets all tasks from a specific plan via MCP
    /// </summary>
    /// <param name="planId">ID of the plan to get tasks from</param>
    /// <param name="hideCompleted">Optional string to hide completed tasks ("true" to hide, anything else to show all)</param>
    /// <returns>JSON string containing all tasks for the specified plan in hierarchical structure</returns>
    [McpServerTool]
    [Description("Gets all tasks from a specific plan with their IDs, titles, descriptions, and status details in hierarchical structure.")]
    public async Task<string> GetTasksFromPlan(
        [Description("ID of the plan to get tasks from")] string planId,
        [Description("Optional string to hide completed tasks (\"true\" to hide completed tasks, anything else shows all)")] string? hideCompleted = null)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        // Parse hideCompleted parameter
        bool shouldHideCompleted = !string.IsNullOrEmpty(hideCompleted) && 
                                   hideCompleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Get all tasks for the plan
        var tasks = await _repository.GetTasksByPlanAsync(planId);
        
        // Filter out completed tasks if requested
        if (shouldHideCompleted)
        {
            tasks = tasks.Where(t => t.Status != Enums.TaskStatus.Completed).ToList();
        }
        
        // Build hierarchical structure
        var hierarchicalTasks = BuildTaskHierarchyForPlan(tasks);
        
        var json = JsonSerializer.Serialize(hierarchicalTasks, _jsonOptions);
        return json;
    }

    /// <summary>
    /// Adds a new Task item via MCP
    /// </summary>
    /// <param name="planId">ID of the project plan</param>
    /// <param name="title">Title/description of the Task</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="tags">Comma-separated tags for grouping</param>
    /// <param name="groups">Comma-separated groups for organizing Tasks (e.g., releases, components)</param>
    /// <param name="parentTaskId">ID of parent Task if this is nested</param>
    /// <param name="estimateHours">Estimated hours for completing this Task</param>
    /// <returns>JSON string containing the created Task item</returns>
    [McpServerTool]
    [Description("Adds a new Task / TODO item to a project plan. 3-levels deep nesting of tasks to compartmentalize complex tasks is recommended for plans.")]
    public async Task<string> AddTaskToPlanAsync(
        [Description("ID of the project plan, as from the create new plan response.")] string planId,
        [Description("Title/description of the Task / TODO that an AI can understand")] string title,
        [Description("Detailed description with important references like docs, rules, restrictions, file & directory paths")] string description,
        [Description("Specific acceptance criteria for this Task - clear, measurable criteria that define when the task is complete")] string acceptanceCriteria,
        [Description("Comma-separated tags for categorizing Tasks. Like dev, test, analysis etc")] string tags,
        [Description("Comma-separated groups for organizing Tasks (e.g., releases, components)")] string groups,
        [Description("ID of parent Task if this is a child of another Task")] string? parentTaskId,
        [Description("Estimated hours for completing this Task")] double estimateHours)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        if (description == null)
            throw new ArgumentNullException(nameof(description));
        if (acceptanceCriteria == null)
            throw new ArgumentNullException(nameof(acceptanceCriteria));
        if (tags == null)
            throw new ArgumentNullException(nameof(tags));
        if (groups == null)
            throw new ArgumentNullException(nameof(groups));
        if (estimateHours < 0)
            throw new ArgumentException("Estimate hours cannot be negative.", nameof(estimateHours));

        var tagList = string.IsNullOrWhiteSpace(tags) ? new List<string>() : tags.Split(',').Select(t => t.Trim()).ToList();
        var groupList = string.IsNullOrWhiteSpace(groups) ? new List<string>() : groups.Split(',').Select(g => g.Trim()).ToList();
        
        var task = new TaskItem
        {
            Id = Guid.NewGuid().ToString(),
            PlanId = planId,
            Title = title,
            Description = description,
            AcceptanceCriteria = acceptanceCriteria,
            Tags = tagList,
            Groups = groupList,
            ParentTaskId = parentTaskId,
            Status = Enums.TaskStatus.Todo,
            EstimateHours = estimateHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _repository.AddTaskAsync(task);
        var json = JsonSerializer.Serialize(task, _jsonOptions);
        return json;
    }

    /// <summary>
    /// Gets a task with all its children in a hierarchical structure via MCP
    /// </summary>
    /// <param name="taskId">ID of the task to retrieve</param>
    /// <returns>JSON string containing the task with its nested children</returns>
    [McpServerTool]
    [Description("Gets a task with all its children in a hierarchical structure. Returns the complete task tree starting from the specified task ID.")]
    public async Task<string> GetTaskWithChildrenByIdAsync(
        [Description("ID of the task to retrieve with its children")] string taskId)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));

        // Get the root task
        var rootTask = await _repository.GetTaskByIdAsync(taskId);
        
        if (rootTask == null)
        {
            var notFoundResult = new { 
                message = "Task not found", 
                taskId, 
                task = (TaskItem?)null 
            };
            return JsonSerializer.Serialize(notFoundResult, _jsonOptions);
        }

        // Get all tasks for the same plan to build the hierarchy
        var allTasks = await _repository.GetTasksByPlanAsync(rootTask.PlanId);
        
        // Build the hierarchical structure starting from the root task
        var taskWithChildren = await BuildTaskHierarchyAsync(rootTask, allTasks);
        
        var result = new { 
            message = "Task retrieved successfully", 
            taskId, 
            task = taskWithChildren 
        };
        
        return JsonSerializer.Serialize(result, _jsonOptions);
    }

    /// <summary>
    /// Marks a task as completed via MCP
    /// </summary>
    /// <param name="taskId">ID of the task to mark as completed</param>
    /// <param name="hasPassedMinimalQualityGates">String indicating that the caller has confirmed the solution builds and all tests pass ("true" to confirm quality gates passed)</param>
    /// <param name="hasAddedCleanupTasks">String indicating that the caller has added the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out etc. ("true" to confirm cleanup tasks added)</param>
    /// <returns>JSON string containing the updated task</returns>
    [McpServerTool]
    [Description("Marks a task as completed and sets the completion timestamp. This tool updates the task status to 'Completed' and records when it was completed.")]
    public async Task<string> MarkTaskAsCompletedAsync(
        [Description("ID of the task to mark as completed")] string taskId,
        [Description("String indicating that the caller has confirmed the entire solution builds successfully and all tests pass before marking task as completed (\"true\" to confirm quality gates passed). You **must be honest and truthful**.")] string hasPassedMinimalQualityGates,
        [Description("String indicating that the caller has added the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out etc. (\"true\" to confirm cleanup tasks added).  You **must be honest and truthful**.")]
        string hasAddedCleanupTasks)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));

        // Parse hasPassedMinimalQualityGates parameter
        bool qualityGatesPassed = !string.IsNullOrEmpty(hasPassedMinimalQualityGates) && 
                                  hasPassedMinimalQualityGates.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Quality gates validation
        if (!qualityGatesPassed)
        {
            var qualityGatesResult = new { 
                message = "Quality gates validation failed. You must ensure the entire solution builds successfully and all tests pass before attempting to mark task as completed. Set hasPassedMinimalQualityGates to \"true\" only after confirming the solution is in a stable state.", 
                taskId, 
                success = false,
                error = "QualityGatesNotMet",
                requirements = new[] 
                {
                    "Solution must build successfully without errors",
                    "All unit tests must pass",
                    "All integration tests must pass",
                    "Code quality checks must pass"
                },
                providedQualityGatesStatus = hasPassedMinimalQualityGates,
                parsedQualityGatesStatus = qualityGatesPassed
            };
            return JsonSerializer.Serialize(qualityGatesResult, _jsonOptions);
        }

        // Parse hasAddedCleanupTasks parameter
        bool cleanupTasksAdded = !string.IsNullOrEmpty(hasAddedCleanupTasks) && 
                                 hasAddedCleanupTasks.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Cleanup tasks validation
        if (!cleanupTasksAdded)
        {
            var cleanupTasksResult = new { 
                message = "Cleanup tasks validation failed. You must add the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out etc. before attempting to mark task as completed. Set hasAddedCleanupTasks to \"true\" only after confirming cleanup tasks have been added.", 
                taskId, 
                success = false,
                error = "CleanupTasksNotAdded",
                requirements = new[] 
                {
                    "All shortcuts must have corresponding cleanup tasks",
                    "Any dummy or mock data usage must have cleanup tasks",
                    "Any commented out code must have cleanup tasks",
                    "Any temporary solutions must have proper implementation tasks"
                },
                providedCleanupTasksStatus = hasAddedCleanupTasks,
                parsedCleanupTasksStatus = cleanupTasksAdded
            };
            return JsonSerializer.Serialize(cleanupTasksResult, _jsonOptions);
        }

        // Get the task
        var task = await _repository.GetTaskByIdAsync(taskId);
        
        if (task == null)
        {
            var notFoundResult = new { 
                message = "Task not found", 
                taskId, 
                success = false,
                task = (TaskItem?)null 
            };
            return JsonSerializer.Serialize(notFoundResult, _jsonOptions);
        }

        // Check if task is already completed
        if (task.Status == Enums.TaskStatus.Completed)
        {
            var alreadyCompletedResult = new { 
                message = "Task is already completed", 
                taskId, 
                success = false,
                task,
                completedAt = task.CompletedAt
            };
            return JsonSerializer.Serialize(alreadyCompletedResult, _jsonOptions);
        }

        // Update task status and completion timestamp
        task.Status = Enums.TaskStatus.Completed;
        task.UpdatedAt = DateTime.UtcNow;
        task.CompletedAt = DateTime.UtcNow;

        // Save the updated task
        await _repository.UpdateTaskAsync(task);

        var successResult = new { 
            message = "Task marked as completed successfully", 
            taskId, 
            success = true,
            task,
            completedAt = task.CompletedAt
        };
        
        return JsonSerializer.Serialize(successResult, _jsonOptions);
    }

    /// <summary>
    /// Updates the status of a task via MCP
    /// </summary>
    /// <param name="taskId">ID of the task to update</param>
    /// <param name="status">New status for the task</param>
    /// <param name="hasPassedMinimalQualityGates">String indicating that the caller has confirmed the solution builds and all tests pass ("true" to confirm quality gates passed)</param>
    /// <returns>JSON string containing the updated task</returns>
    // [McpServerTool]
    [Description("Updates the status of a task. Available statuses are: Todo, InProgress, Completed, Blocked, Cancelled. When marking as Completed, automatically sets completion timestamp.")]
    public async Task<string> UpdateTaskStatusAsync(
        [Description("ID of the task to update")] string taskId,
        [Description("New status for the task (Todo, InProgress, Completed, Blocked, Cancelled)")] string status,
        [Description("String indicating that the caller has confirmed the entire solution builds successfully and all tests pass before changing task status (\"true\" to confirm quality gates passed)")] string hasPassedMinimalQualityGates)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be null or empty.", nameof(status));

        // Parse hasPassedMinimalQualityGates parameter
        bool qualityGatesPassed = !string.IsNullOrEmpty(hasPassedMinimalQualityGates) && 
                                  hasPassedMinimalQualityGates.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Quality gates validation
        if (!qualityGatesPassed)
        {
            var qualityGatesResult = new { 
                message = "Quality gates validation failed. You must ensure the entire solution builds successfully and all tests pass before attempting to change task status. Set hasPassedMinimalQualityGates to \"true\" only after confirming the solution is in a stable state.", 
                taskId, 
                success = false,
                error = "QualityGatesNotMet",
                requirements = new[] 
                {
                    "Solution must build successfully without errors",
                    "All unit tests must pass",
                    "All integration tests must pass",
                    "Code quality checks must pass"
                },
                providedQualityGatesStatus = hasPassedMinimalQualityGates,
                parsedQualityGatesStatus = qualityGatesPassed
            };
            return JsonSerializer.Serialize(qualityGatesResult, _jsonOptions);
        }

        // Parse and validate status
        if (!Enum.TryParse<Enums.TaskStatus>(status, true, out var taskStatus))
        {
            var invalidStatusResult = new { 
                message = $"Invalid status '{status}'. Valid statuses are: {string.Join(", ", Enum.GetNames<Enums.TaskStatus>())}", 
                taskId, 
                success = false,
                providedStatus = status,
                validStatuses = Enum.GetNames<Enums.TaskStatus>()
            };
            return JsonSerializer.Serialize(invalidStatusResult, _jsonOptions);
        }

        // Get the task
        var task = await _repository.GetTaskByIdAsync(taskId);
        
        if (task == null)
        {
            var notFoundResult = new { 
                message = "Task not found", 
                taskId, 
                success = false,
                task = (TaskItem?)null 
            };
            return JsonSerializer.Serialize(notFoundResult, _jsonOptions);
        }

        var previousStatus = task.Status;
        var wasCompleted = task.Status == Enums.TaskStatus.Completed;

        // Update task status and timestamps
        task.Status = taskStatus;
        task.UpdatedAt = DateTime.UtcNow;

        // Handle completion timestamp logic
        if (taskStatus == Enums.TaskStatus.Completed && !wasCompleted)
        {
            // Task is being marked as completed for the first time
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (taskStatus != Enums.TaskStatus.Completed && wasCompleted)
        {
            // Task is being moved away from completed status
            task.CompletedAt = null;
        }
        // If already completed and staying completed, keep existing CompletedAt

        // Save the updated task
        await _repository.UpdateTaskAsync(task);

        var successResult = new { 
            message = $"Task status updated successfully from '{previousStatus}' to '{taskStatus}'", 
            taskId, 
            success = true,
            task,
            previousStatus = previousStatus.ToString(),
            newStatus = taskStatus.ToString(),
            completedAt = task.CompletedAt
        };
        
        return JsonSerializer.Serialize(successResult, _jsonOptions);
    }

    /// <summary>
    /// Recursively builds the task hierarchy by populating children
    /// </summary>
    /// <param name="parentTask">The parent task to populate children for</param>
    /// <param name="allTasks">All tasks in the plan</param>
    /// <returns>Task with populated children hierarchy</returns>
    private async Task<TaskItem> BuildTaskHierarchyAsync(TaskItem parentTask, List<TaskItem> allTasks)
    {
        // Create a copy of the parent task to avoid modifying the original
        var taskWithChildren = new TaskItem
        {
            Id = parentTask.Id,
            PlanId = parentTask.PlanId,
            ParentTaskId = parentTask.ParentTaskId,
            Title = parentTask.Title,
            Description = parentTask.Description,
            AcceptanceCriteria = parentTask.AcceptanceCriteria,
            Status = parentTask.Status,
            Tags = new List<string>(parentTask.Tags),
            Groups = new List<string>(parentTask.Groups),
            EstimateHours = parentTask.EstimateHours,
            CreatedAt = parentTask.CreatedAt,
            UpdatedAt = parentTask.UpdatedAt,
            CompletedAt = parentTask.CompletedAt,
            Children = new List<TaskItem>()
        };

        // Find all direct children of this task
        var children = allTasks.Where(t => t.ParentTaskId == parentTask.Id).ToList();
        
        // Recursively build hierarchy for each child
        foreach (var child in children)
        {
            var childWithHierarchy = await BuildTaskHierarchyAsync(child, allTasks);
            taskWithChildren.Children.Add(childWithHierarchy);
        }
        
        // Sort children by creation date for consistent ordering
        taskWithChildren.Children = taskWithChildren.Children.OrderBy(c => c.CreatedAt).ToList();
        
        return taskWithChildren;
    }

    /// <summary>
    /// Builds hierarchical structure for a list of tasks
    /// </summary>
    /// <param name="tasks">Flat list of tasks</param>
    /// <returns>List of hierarchical task objects with nested children</returns>
    private List<object> BuildTaskHierarchyForPlan(List<TaskItem> tasks)
    {
        // Create a dictionary for fast lookup
        var taskDict = tasks.ToDictionary(t => t.Id, t => new
        {
            id = t.Id,
            planId = t.PlanId,
            title = t.Title,
            description = t.Description,
            acceptanceCriteria = t.AcceptanceCriteria,
            tags = t.Tags,
            groups = t.Groups,
            parentTaskId = t.ParentTaskId,
            status = t.Status.ToString(),
            estimateHours = t.EstimateHours,
            createdAt = t.CreatedAt,
            updatedAt = t.UpdatedAt,
            completedAt = t.CompletedAt,
            children = new List<object>()
        });
        
        var rootTasks = new List<object>();
        
        // Build the hierarchy
        foreach (var task in tasks)
        {
            if (string.IsNullOrEmpty(task.ParentTaskId))
            {
                // This is a root task
                if (taskDict.TryGetValue(task.Id, out var rootTask))
                {
                    rootTasks.Add(rootTask);
                }
            }
            else
            {
                // This is a child task
                if (taskDict.TryGetValue(task.Id, out var childTask) && 
                    taskDict.TryGetValue(task.ParentTaskId, out var parentTask))
                {
                    ((List<object>)parentTask.children).Add(childTask);
                }
                else if (taskDict.TryGetValue(task.Id, out var orphanTask))
                {
                    // Parent not found - treat as root task (orphaned)
                    rootTasks.Add(orphanTask);
                }
            }
        }
        
        // Sort root tasks by creation date for consistent ordering
        rootTasks = rootTasks.OrderBy(t => ((dynamic)t).createdAt).ToList();
        
        // Recursively sort children by creation date
        SortChildrenRecursively(rootTasks);
        
        return rootTasks;
    }

    /// <summary>
    /// Recursively sorts children by creation date for consistent ordering
    /// </summary>
    /// <param name="tasks">List of task objects to sort children for</param>
    private void SortChildrenRecursively(List<object> tasks)
    {
        foreach (var task in tasks)
        {
            var children = (List<object>)((dynamic)task).children;
            if (children.Any())
            {
                var sortedChildren = children.OrderBy(c => ((dynamic)c).createdAt).ToList();
                children.Clear();
                children.AddRange(sortedChildren);
                
                // Recursively sort grandchildren
                SortChildrenRecursively(children);
            }
        }
    }
}
