using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for task planner repository operations
/// </summary>
public interface ITaskPlannerRepository
{
    /// <summary>
    /// Adds a new project plan
    /// </summary>
    /// <param name="plan">The plan to add</param>
    /// <returns>The added plan</returns>
    Task<ProjectPlan> AddPlanAsync(ProjectPlan plan);

    /// <summary>
    /// Adds a new Task item to the database
    /// </summary>
    /// <param name="task">The Task item to add</param>
    /// <returns>Task representing the operation</returns>
    Task AddTaskAsync(TaskItem task);

    /// <summary>
    /// Gets a project plan by ID
    /// </summary>
    /// <param name="planId">ID of the plan to retrieve</param>
    /// <returns>The plan or null if not found</returns>
    Task<ProjectPlan?> GetPlanByIdAsync(string planId);

    /// <summary>
    /// Gets all project plans
    /// </summary>
    /// <returns>List of all plans</returns>
    Task<List<ProjectPlan>> GetAllPlansAsync();

    /// <summary>
    /// Gets all Task items for a plan
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>List of Task items for the plan</returns>
    Task<List<TaskItem>> GetTasksByPlanAsync(string planId);

    /// <summary>
    /// Gets a Task item by ID
    /// </summary>
    /// <param name="taskId">ID of the Task item</param>
    /// <returns>Task item or null if not found</returns>
    Task<TaskItem?> GetTaskByIdAsync(string taskId);

    /// <summary>
    /// Updates a Task item in the database
    /// </summary>
    /// <param name="task">Updated Task item</param>
    /// <returns>Task representing the operation</returns>
    Task UpdateTaskAsync(TaskItem task);
}
