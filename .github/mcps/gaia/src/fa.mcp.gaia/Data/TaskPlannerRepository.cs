using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// Repository for task planner operations with proper error handling
/// </summary>
public class TaskPlannerRepository : ITaskPlannerRepository
{
    private readonly TaskPlannerDbContext _dbContext;
    private readonly IWebhookRepository _webhookRepository;

    public TaskPlannerRepository(TaskPlannerDbContext dbContext, IWebhookRepository webhookRepository)
    {
        _dbContext = dbContext;
        _webhookRepository = webhookRepository;
    }

    /// <summary>
    /// Adds a new project plan
    /// </summary>
    /// <param name="plan">The plan to add</param>
    /// <returns>The added plan</returns>
    public async Task<ProjectPlan> AddPlanAsync(ProjectPlan plan)
    {
        await _dbContext.AddPlanAsync(plan);
        
        // Send webhook notification
        await _webhookRepository.SendPlanWebhookAsync(plan);
        
        return plan;
    }

    /// <summary>
    /// Adds a new Task item to the database
    /// </summary>
    /// <param name="task">Task item to add</param>
    public async Task AddTaskAsync(TaskItem task)
    {
        await _dbContext.AddTaskAsync(task);
        
        // Send webhook notification with the full plan
        var plan = await _dbContext.GetPlanByIdAsync(task.PlanId);
        if (plan != null)
        {
            await _webhookRepository.SendPlanWebhookAsync(plan);
        }
    }

    /// <summary>
    /// Gets a project plan by ID
    /// </summary>
    /// <param name="planId">ID of the plan to retrieve</param>
    /// <returns>The plan or null if not found</returns>
    public async Task<ProjectPlan?> GetPlanByIdAsync(string planId)
    {
        return await _dbContext.GetPlanByIdAsync(planId);
    }

    /// <summary>
    /// Gets all project plans
    /// </summary>
    /// <returns>List of all plans</returns>
    public async Task<List<ProjectPlan>> GetAllPlansAsync()
    {
        return await _dbContext.GetAllPlansAsync();
    }

    /// <summary>
    /// Gets all Task items for a plan (returns flat list for performance)
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>List of Task items for the plan</returns>
    public async Task<List<TaskItem>> GetTasksByPlanAsync(string planId)
    {
        var tasks = await _dbContext.GetTasksByPlanIdAsync(planId);
        
        return tasks;
    }

    /// <summary>
    /// Builds hierarchical structure for Tasks (for display purposes)
    /// </summary>
    /// <param name="flatTasks">Flat list of Tasks</param>
    /// <returns>List of root Tasks with children populated</returns>
    private List<TaskItem> BuildTaskHierarchy(List<TaskItem> flatTasks)
    {
        // Create a dictionary for fast lookup
        var taskDict = flatTasks.ToDictionary(t => t.Id, t => t);
        
        // Clear existing children (in case this is called multiple times)
        foreach (var task in flatTasks)
        {
            task.Children.Clear();
        }
        
        // Build the hierarchy
        var rootTasks = new List<TaskItem>();
        
        foreach (var task in flatTasks)
        {
            if (string.IsNullOrEmpty(task.ParentTaskId))
            {
                // This is a root Task
                rootTasks.Add(task);
            }
            else
            {
                // This is a child Task
                if (taskDict.TryGetValue(task.ParentTaskId, out var parent))
                {
                    parent.Children.Add(task);
                }
                else
                {
                    // Parent not found - treat as root Task (orphaned)
                    rootTasks.Add(task);
                }
            }
        }
        
        return rootTasks;
    }

    /// <summary>
    /// Gets a Task item by ID
    /// </summary>
    /// <param name="taskId">ID of the Task item</param>
    /// <returns>Task item or null if not found</returns>
    public async Task<TaskItem?> GetTaskByIdAsync(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));
        }

        var task = await _dbContext.GetTaskByIdAsync(taskId);
        
        return task;
    }

    /// <summary>
    /// Updates a Task item in the database
    /// </summary>
    /// <param name="task">Updated Task item</param>
    public async Task UpdateTaskAsync(TaskItem task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        await _dbContext.UpdateTaskAsync(task);
        
        // Send webhook notification with the full plan
        var plan = await _dbContext.GetPlanByIdAsync(task.PlanId);
        if (plan != null)
        {
            await _webhookRepository.SendPlanWebhookAsync(plan);
        }
    }
}
