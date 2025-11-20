using System.ComponentModel;
using System.Text.Json;
using FrostAura.MCP.Gaia.Configuration;
using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for Task planning operations with integrated MCP tools
/// </summary>
[McpServerToolType]
public class TaskPlannerManager : ITaskPlannerManager
{
    private readonly ITaskPlannerRepository _repository;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<TaskPlannerManager>? _logger;

    /// <summary>
    /// Initializes a new instance of the TaskPlannerManager
    /// </summary>
    /// <param name="repository">Task repository</param>
    /// <param name="configuration">Configuration to check for error handling preferences</param>
    /// <param name="logger">Logger for debugging and error tracking (optional)</param>
    public TaskPlannerManager(ITaskPlannerRepository repository, IConfiguration configuration, ILogger<TaskPlannerManager>? logger = null)
    {
        _repository = repository;
        _jsonOptions = JsonConfiguration.GetApiOptions();
        _logger = logger;
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
    public async Task<string> CreateNewPlanAsync(
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
    /// <param name="hideCompletedPlansAndTasks">Whether to hide completed plans. Note: Task filtering is not applicable as tasks are not returned.</param>
    /// <returns>JSON string containing all project plans with basic information</returns>
    [McpServerTool]
    [Description("Lists all project plans with their IDs, names, descriptions, progress metrics, and calculated status. Does not include tasks.")]
    public async Task<string> ListPlansAsync(
        [Description("Whether to hide completed plans. Note: Task filtering is not applicable as tasks are not returned.")] bool hideCompletedPlansAndTasks)
    {
        // Get all plans
        var plans = await _repository.GetAllPlansAsync();

        // Filter completed plans if requested
        if (hideCompletedPlansAndTasks)
        {
            // Get all tasks for all plans in parallel
            var taskQueries = plans.Select(plan =>
                _repository.GetTasksByPlanAsync(plan.Id)).ToArray();
            var allPlanTasks = await Task.WhenAll(taskQueries);

            var filteredPlans = new List<ProjectPlan>();

            for (int i = 0; i < plans.Count; i++)
            {
                var plan = plans[i];
                var tasks = allPlanTasks[i];

                // A plan is considered completed if it has tasks and all tasks are completed
                // Plans with no tasks are considered not completed (still in planning phase)
                var isCompleted = tasks.Any() && tasks.All(t => t.Status == Enums.TaskStatus.Completed);

                if (!isCompleted)
                {
                    filteredPlans.Add(plan);
                }
            }

            plans = filteredPlans;
        }

        // Create plan summaries with basic information only
        var planSummaries = plans.Select(plan => new
        {
            id = plan.Id,
            name = plan.Name,
            description = plan.Description,
            aiAgentBuildContext = plan.AiAgentBuildContext,
            creatorIdentity = plan.CreatorIdentity,
            estimateHours = plan.EstimateHours,
            createdAt = plan.CreatedAt,
            updatedAt = plan.UpdatedAt
        }).ToList();

        var json = JsonSerializer.Serialize(planSummaries, _jsonOptions);
        return json;
    }

    /// <summary>
    /// Gets all tasks from a specific plan via MCP
    /// </summary>
    /// <param name="planId">ID of the plan to get tasks from</param>
    /// <param name="hideCompletedTasks">Whether to hide completed tasks. Ideal for only fetching plans with outstanding tasks.</param>
    /// <returns>JSON string containing all tasks for the specified plan in hierarchical structure</returns>
    [McpServerTool]
    [Description("Gets all tasks from a specific plan with their IDs, titles, descriptions, and status details in hierarchical structure.")]
    public async Task<string> GetTasksFromPlan(
        [Description("ID of the plan to get tasks from")] string planId,
        [Description("Whether to hide completed tasks. Ideal for only fetching plans with outstanding tasks.")] bool hideCompletedTasks = true)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        // Get all tasks for the plan
        var tasks = await _repository.GetTasksByPlanAsync(planId);

        // Build hierarchical structure first to maintain parent-child relationships
        var hierarchicalTasks = BuildTaskHierarchyForPlan(tasks);

        // Filter out completed tasks if requested, while preserving hierarchy integrity
        if (hideCompletedTasks)
        {
            hierarchicalTasks = FilterCompletedTasksFromHierarchy(hierarchicalTasks);
        }

        var json = JsonSerializer.Serialize(hierarchicalTasks, _jsonOptions);
        return json;
    }

    /// <summary>
    /// Adds a new Task item via MCP
    /// </summary>
    /// <param name="planId">ID of the project plan</param>
    /// <param name="title">Title/description of the Task</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="owner">Agent responsible for this Task</param>
    /// <param name="tags">Comma-separated tags for grouping</param>
    /// <param name="groups">Comma-separated groups for organizing Tasks (e.g., releases, components)</param>
    /// <param name="parentTaskId">ID of parent Task if this is nested</param>
    /// <param name="estimateHours">Estimated hours for completing this Task</param>
    /// <returns>JSON string containing the created Task item</returns>
    [McpServerTool]
    [Description("Adds a new Task / TODO item to a project plan. 3-levels deep nesting of tasks to compartmentalize complex tasks is recommended for plans.")]
    public async Task<string> AddNewTaskToPlanAsync(
        [Description("ID of the project plan, as from the create new plan response.")] string planId,
        [Description("Title/description of the Task / TODO that an AI can understand")] string title,
        [Description("Detailed description with important references like docs, rules, restrictions, file & directory paths")] string description,
        [Description("Specific acceptance criteria for this Task - clear, measurable criteria that define when the task is complete")] string acceptanceCriteria,
        [Description("Agent responsible for this Task (e.g., Code-Implementer, Unit-Tester, Design-Architect)")] string owner,
        [Description("Comma-separated tags for categorizing Tasks. Like dev, test, analysis etc")] string tags,
        [Description("Comma-separated groups for organizing Tasks (e.g., releases, components)")] string groups,
        [Description("ID of parent Task if this is a child of another Task")] string? parentTaskId,
        [Description("Estimated hours for completing this Task")] double estimateHours)
    {
        try
        {
            _logger?.LogInformation("=== AddTaskToPlanAsync ===");
            _logger?.LogInformation("planId: '{PlanId}'", planId);
            _logger?.LogInformation("title: '{Title}'", title);
            _logger?.LogInformation("description: '{Description}'", description);
            _logger?.LogInformation("acceptanceCriteria: '{AcceptanceCriteria}'", acceptanceCriteria);
            _logger?.LogInformation("owner: '{Owner}'", owner);
            _logger?.LogInformation("tags: '{Tags}'", tags);
            _logger?.LogInformation("groups: '{Groups}'", groups);
            _logger?.LogInformation("parentTaskId: '{ParentTaskId}' (IsNull: {IsNull}, IsEmpty: {IsEmpty})",
                parentTaskId, parentTaskId == null, string.IsNullOrEmpty(parentTaskId));
            _logger?.LogInformation("estimateHours: {EstimateHours}", estimateHours);

            // Input validation
            if (string.IsNullOrWhiteSpace(planId))
                throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            if (description == null)
                throw new ArgumentNullException(nameof(description));
            if (acceptanceCriteria == null)
                throw new ArgumentNullException(nameof(acceptanceCriteria));
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner cannot be null or empty.", nameof(owner));
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));
            if (groups == null)
                throw new ArgumentNullException(nameof(groups));
            if (estimateHours < 0)
                throw new ArgumentException("Estimate hours cannot be negative.", nameof(estimateHours));

            // Convert empty string to null for parentTaskId
            var cleanParentTaskId = string.IsNullOrWhiteSpace(parentTaskId) ? null : parentTaskId;
            _logger?.LogInformation("Cleaned parentTaskId: '{CleanParentTaskId}' (Original: '{OriginalParentTaskId}')",
                cleanParentTaskId, parentTaskId);

            var tagList = string.IsNullOrWhiteSpace(tags) ? new List<string>() : tags.Split(',').Select(t => t.Trim()).ToList();
            var groupList = string.IsNullOrWhiteSpace(groups) ? new List<string>() : groups.Split(',').Select(g => g.Trim()).ToList();

            var task = new TaskItem
            {
                Id = Guid.NewGuid().ToString(),
                PlanId = planId,
                Title = title,
                Description = description,
                AcceptanceCriteria = acceptanceCriteria,
                Owner = owner,
                Tags = tagList,
                Groups = groupList,
                ParentTaskId = cleanParentTaskId,
                Status = Enums.TaskStatus.Todo,
                EstimateHours = estimateHours,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger?.LogInformation("Created TaskItem object:");
            _logger?.LogInformation("  Id: {Id}", task.Id);
            _logger?.LogInformation("  PlanId: {PlanId}", task.PlanId);
            _logger?.LogInformation("  ParentTaskId: '{ParentTaskId}' (IsNull: {IsNull})", task.ParentTaskId, task.ParentTaskId == null);
            _logger?.LogInformation("  Title: {Title}", task.Title);

            _logger?.LogInformation("Calling repository AddTaskAsync...");
            await _repository.AddTaskAsync(task);
            _logger?.LogInformation("Repository AddTaskAsync completed successfully.");

            var json = JsonSerializer.Serialize(task, _jsonOptions);
            _logger?.LogInformation("Serialized task JSON: {Json}", json);

            return json;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "AddTaskToPlanAsync failed with exception");

            var errorResponse = new
            {
                success = false,
                error = "AddTaskToPlan_Exception",
                message = ex.Message,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.ToString(),
                parameters = new
                {
                    planId,
                    title,
                    description,
                    acceptanceCriteria,
                    owner,
                    tags,
                    groups,
                    parentTaskId,
                    estimateHours
                }
            };

            var errorJson = JsonSerializer.Serialize(errorResponse, _jsonOptions);
            _logger?.LogError("Error response JSON: {ErrorJson}", errorJson);
            return errorJson;
        }
    }

    /// <summary>
    /// Marks a task as completed and sets the completion timestamp. This tool updates the task status to 'Completed' and records when it was completed.
    /// </summary>
    /// <param name="taskId">ID of the task to mark as completed.</param>
    /// <param name="hasValidatedThinkingPriorToCompletion">Whether you have used your CritiqueThought tool prior to considering the task completed.This is **mandatory**.</param>
    /// <param name="hasFullUnitTestsCoverageWithAllPassingTests">Whether you have confirmed the entire solution builds successfully, we optained **100% test coverage** of the task **and** entire solution and all tests pass before marking task as completed. `npm run test:unit` should be leveraged here or the available NPM command for testing, for this project.</param>
    /// <param name="hasAddedCleanupTasks">Whether the caller has added the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out, temporary files created etc. These tasks are typically nested tasks (level-2 tasks), associated with the larger task.</param>
    /// <returns>JSON string containing the updated task</returns>
    [McpServerTool]
    [Description($@"
        - Marks a task as completed and sets the completion timestamp. This tool updates the task status to 'Completed' and records when it was completed.
        - You **must be honest and truthful**. For each of the parameters that require bools, you must ensure you perform the applicable tasks in order to be allowed to proceed with completion.
            - You must not take shortcuts.
            - You must not care about what work is scoped to the ticket, all checks must pass regardless.
            - You must provide a value for all parameters, truthfully. **If not all parameters are provided, the tool is expected to produce an error.
    ")]
    public async Task<string> MarkTaskAsCompletedAsync(
        [Description("ID of the task to mark as completed.")] string taskId,
        [Description($"Whether you have used your {nameof(ThinkingManager.DoubleCheckThoughtAsync)} tool prior to considering the task completed. This is **mandatory** not just for when you struggle but for general feedback. Consider the {nameof(ThinkingManager.DoubleCheckThoughtAsync)} tool a reviewer.")] bool hasValidatedThinkingPriorToCompletion,
        [Description("Whether you have confirmed the entire solution builds successfully, we optained **100% test coverage** of the task **and** entire solution and all tests pass before marking task as completed. `npm run test:unit` should be leveraged here or the available NPM command for testing, for this project.")] bool hasFullUnitTestsCoverageWithAllPassingTests,
        [Description("Whether the caller has added the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out, temporary files created etc. These tasks are typically nested tasks (level-2 tasks), associated with the larger task.")]
        bool hasAddedCleanupTasks)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));

        // Quality gates validation
        if (!hasFullUnitTestsCoverageWithAllPassingTests)
        {
            var qualityGatesResult = new
            {
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
                hasFullUnitTestsCoverageWithAllPassingTests
            };
            return JsonSerializer.Serialize(qualityGatesResult, _jsonOptions);
        }

        // Cleanup tasks validation
        if (!hasAddedCleanupTasks)
        {
            var cleanupTasksResult = new
            {
                message = "Cleanup tasks validation failed. You must add the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out etc. before attempting to mark task as completed.",
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
                hasAddedCleanupTasks
            };
            return JsonSerializer.Serialize(cleanupTasksResult, _jsonOptions);
        }

        // Get the task
        var task = await _repository.GetTaskByIdAsync(taskId);

        if (task == null)
        {
            var notFoundResult = new
            {
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
            var alreadyCompletedResult = new
            {
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

        var successResult = new
        {
            message = "Task marked as completed successfully",
            taskId,
            success = true,
            task,
            completedAt = task.CompletedAt
        };

        return JsonSerializer.Serialize(successResult, _jsonOptions);
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
            owner = t.Owner,
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
    /// Filters completed tasks from hierarchical structure while preserving parent-child relationships
    /// </summary>
    /// <param name="tasks">Hierarchical task list to filter</param>
    /// <returns>Filtered hierarchical task list with completed tasks removed</returns>
    private List<object> FilterCompletedTasksFromHierarchy(List<object> tasks)
    {
        var filteredTasks = new List<object>();

        foreach (var task in tasks)
        {
            var taskStatus = ((dynamic)task).status;
            var children = (List<object>)((dynamic)task).children;

            // Recursively filter children first
            var filteredChildren = FilterCompletedTasksFromHierarchy(children);

            // Include task if it's not completed OR if it has non-completed children
            if (taskStatus != "Completed" || filteredChildren.Any())
            {
                // Create a new object with filtered children
                var filteredTask = new
                {
                    id = ((dynamic)task).id,
                    planId = ((dynamic)task).planId,
                    title = ((dynamic)task).title,
                    description = ((dynamic)task).description,
                    acceptanceCriteria = ((dynamic)task).acceptanceCriteria,
                    owner = ((dynamic)task).owner,
                    tags = ((dynamic)task).tags,
                    groups = ((dynamic)task).groups,
                    parentTaskId = ((dynamic)task).parentTaskId,
                    status = ((dynamic)task).status,
                    estimateHours = ((dynamic)task).estimateHours,
                    createdAt = ((dynamic)task).createdAt,
                    updatedAt = ((dynamic)task).updatedAt,
                    completedAt = ((dynamic)task).completedAt,
                    children = filteredChildren
                };

                filteredTasks.Add(filteredTask);
            }
        }

        return filteredTasks;
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
