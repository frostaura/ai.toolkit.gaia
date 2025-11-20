namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for task planner management operations - MCP server tools
/// </summary>
public interface ITaskPlannerManager
{
    /// <summary>
    /// Creates a new project plan via MCP
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description that an AI can understand</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="creatorIdentity">A best attempt at a derived user name / context, typically from the host machine details</param>
    /// <returns>JSON string containing the created project plan</returns>
    Task<string> CreateNewPlanAsync(string projectName, string description, string aiAgentBuildContext, string creatorIdentity);

    /// <summary>
    /// Lists all project plans via MCP
    /// </summary>
    /// <param name="hideCompletedPlansAndTasks">Whether to hide completed plans & tasks. Ideal for only fetching plans with outstanding tasks.</param>
    /// <returns>JSON string containing all project plans with their status and progress information</returns>
    Task<string> ListPlansAsync(bool hideCompletedPlansAndTasks);

    /// <summary>
    /// Gets all tasks from a specific plan via MCP
    /// </summary>
    /// <param name="planId">ID of the plan to get tasks from</param>
    /// <param name="hideCompletedTasks">Whether to hide completed tasks. Ideal for only fetching plans with outstanding tasks.</param>
    /// <returns>JSON string containing all tasks for the specified plan in hierarchical structure</returns>
    Task<string> GetTasksFromPlan(string planId, bool hideCompletedTasks);

    /// <summary>
    /// Adds a new Task item via MCP
    /// </summary>
    /// <param name="planId">ID of the project plan</param>
    /// <param name="title">Title/description of the Task</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="acceptanceCriteria">Specific acceptance criteria for this Task</param>
    /// <param name="tags">Comma-separated tags for grouping</param>
    /// <param name="groups">Comma-separated groups for organizing Tasks (e.g., releases, components)</param>
    /// <param name="parentTaskId">ID of parent Task if this is nested</param>
    /// <param name="estimateHours">Estimated hours for completing this Task</param>
    /// <returns>JSON string containing the created Task item</returns>
    Task<string> AddNewTaskToPlanAsync(string planId, string title, string description, string acceptanceCriteria, string tags, string groups, string? parentTaskId, double estimateHours);



    /// <summary>
    /// Marks a task as completed and sets the completion timestamp. This tool updates the task status to 'Completed' and records when it was completed.
    /// </summary>
    /// <param name="taskId">ID of the task to mark as completed.</param>
    /// <param name="hasValidatedThinkingPriorToCompletion">Whether you have used your CritiqueThought tool prior to considering the task completed.This is **mandatory**.</param>
    /// <param name="hasFullUnitTestsCoverageWithAllPassingTests">Whether you have confirmed the entire solution builds successfully, we optained **100% test coverage** of the task **and** entire solution and all tests pass before marking task as completed. `npm run test:unit` should be leveraged here or the available NPM command for testing, for this project.</param>
    /// <param name="hasAddedCleanupTasks">Whether the caller has added the necessary cleanup tasks for any potential shortcuts taken, dummy or mock data used, code commented out, temporary files created etc. These tasks are typically nested tasks (level-2 tasks), associated with the larger task.</param>
    /// <returns>JSON string containing the updated task</returns>
    Task<string> MarkTaskAsCompletedAsync(
        string taskId,
        bool hasValidatedThinkingPriorToCompletion,
        bool hasFullUnitTestsCoverageWithAllPassingTests,
        bool hasAddedCleanupTasks);
}
