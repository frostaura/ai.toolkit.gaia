using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;
using FrostAura.MCP.Gaia.Configuration;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// JSON database context for Task data with hierarchical structure
/// </summary>
public class TaskPlannerDbContext
{
    private readonly string _databasePath;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the TaskPlannerDbContext
    /// </summary>
    /// <param name="configuration">Configuration instance</param>
    public TaskPlannerDbContext(IConfiguration configuration)
    {
        _databasePath = configuration["TaskPlanner:DatabasePath"] ?? throw new ArgumentNullException("TaskPlanner:DatabasePath configuration is required");
        _jsonOptions = JsonConfiguration.GetStandardOptions();
    }

    /// <summary>
    /// Represents the database structure - plans with nested tasks
    /// </summary>
    private class TaskDatabase
    {
        public List<ProjectPlan> Plans { get; set; } = new List<ProjectPlan>();
        // Tasks are now nested within plans, no separate Tasks collection needed
    }

    /// <summary>
    /// Gets the database content
    /// </summary>
    /// <returns>Database content</returns>
    private async Task<TaskDatabase> GetDatabaseAsync()
    {
        if (!File.Exists(_databasePath))
        {
            await CreateDatabaseFileAsync();
            return new TaskDatabase();
        }

        var jsonContent = await File.ReadAllTextAsync(_databasePath);
        
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return new TaskDatabase();
        }

        var database = JsonSerializer.Deserialize<TaskDatabase>(jsonContent, _jsonOptions);
        return database ?? new TaskDatabase();
    }

    /// <summary>
    /// Saves the database content
    /// </summary>
    /// <param name="database">Database content to save</param>
    private async Task SaveDatabaseAsync(TaskDatabase database)
    {
        // Ensure directory exists
        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var jsonContent = JsonSerializer.Serialize(database, _jsonOptions);
        await File.WriteAllTextAsync(_databasePath, jsonContent);
    }

    /// <summary>
    /// Creates an empty database file
    /// </summary>
    private async Task CreateDatabaseFileAsync()
    {
        await SaveDatabaseAsync(new TaskDatabase());
    }

    /// <summary>
    /// Adds a new project plan
    /// </summary>
    /// <param name="plan">Project plan to add</param>
    public async Task AddPlanAsync(ProjectPlan plan)
    {
        var database = await GetDatabaseAsync();
        
        if (database.Plans.Any(s => s.Id == plan.Id))
        {
            throw new InvalidOperationException($"A plan with ID '{plan.Id}' already exists.");
        }

        database.Plans.Add(plan);
        await SaveDatabaseAsync(database);
    }

    /// <summary>
    /// Adds a new Task item
    /// </summary>
    /// <param name="task">Task to add</param>
    public async Task AddTaskAsync(TaskItem task)
    {
        var database = await GetDatabaseAsync();
        
        // Find the plan this task belongs to
        var plan = database.Plans.FirstOrDefault(p => p.Id == task.PlanId);
        if (plan == null)
        {
            throw new ArgumentException($"Plan with ID '{task.PlanId}' not found.");
        }

        // Get all tasks in the plan (flatten hierarchy to check for duplicates)
        var allTasks = GetAllTasksFromPlan(plan);
        if (allTasks.Any(t => t.Id == task.Id))
        {
            throw new InvalidOperationException($"A task with ID '{task.Id}' already exists.");
        }

        // Add task to appropriate location in hierarchy
        if (string.IsNullOrEmpty(task.ParentTaskId))
        {
            // Root level task - add directly to plan
            plan.Tasks.Add(task);
        }
        else
        {
            // Child task - find parent and add to its children
            var parentTask = FindTaskInPlan(plan, task.ParentTaskId);
            if (parentTask == null)
            {
                throw new ArgumentException($"Parent task with ID '{task.ParentTaskId}' not found.");
            }
            parentTask.Children.Add(task);
        }

        await SaveDatabaseAsync(database);
    }

    /// <summary>
    /// Gets a project plan by ID with hierarchical Tasks populated
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>Project plan with Tasks or null if not found</returns>
    public async Task<ProjectPlan?> GetPlanByIdAsync(string planId)
    {
        var database = await GetDatabaseAsync();
        var plan = database.Plans.FirstOrDefault(s => s.Id == planId);
        
        if (plan == null)
        {
            return null;
        }

        // Tasks are already hierarchically structured in the plan
        return plan;
    }

    /// <summary>
    /// Gets all project plans with hierarchical Tasks populated
    /// </summary>
    /// <returns>List of project plans with Tasks</returns>
    public async Task<List<ProjectPlan>> GetAllPlansAsync()
    {
        var database = await GetDatabaseAsync();
        
        // Plans already have their tasks hierarchically structured
        return database.Plans;
    }

    /// <summary>
    /// Gets Task items for a specific plan
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>List of Task items for the plan</returns>
    public async Task<List<TaskItem>> GetTasksByPlanIdAsync(string planId)
    {
        var database = await GetDatabaseAsync();
        var plan = database.Plans.FirstOrDefault(p => p.Id == planId);
        
        if (plan == null)
        {
            return new List<TaskItem>();
        }

        // Return flattened list of all tasks in the plan
        return GetAllTasksFromPlan(plan);
    }

    /// <summary>
    /// Gets a Task item by ID
    /// </summary>
    /// <param name="taskId">ID of the Task item</param>
    /// <returns>Task item or null if not found</returns>
    public async Task<TaskItem?> GetTaskByIdAsync(string taskId)
    {
        var database = await GetDatabaseAsync();
        
        // Search across all plans for the task
        foreach (var plan in database.Plans)
        {
            var task = FindTaskInPlan(plan, taskId);
            if (task != null) return task;
        }
        
        return null;
    }

    /// <summary>
    /// Builds hierarchical structure for Tasks
    /// </summary>
    /// <param name="flatTasks">Flat list of Tasks</param>
    /// <returns>List of root Tasks with children populated</returns>
    private List<TaskItem> BuildTaskHierarchy(List<TaskItem> flatTasks)
    {
        var taskDict = flatTasks.ToDictionary(t => t.Id, t => 
            new TaskItem
            {
                Id = t.Id,
                PlanId = t.PlanId,
                ParentTaskId = t.ParentTaskId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Tags = new List<string>(t.Tags),
                Groups = new List<string>(t.Groups),
                EstimateHours = t.EstimateHours,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CompletedAt = t.CompletedAt,
                Children = new List<TaskItem>()
            });
        
        var rootTasks = new List<TaskItem>();
        
        foreach (var task in taskDict.Values)
        {
            if (string.IsNullOrEmpty(task.ParentTaskId))
            {
                rootTasks.Add(task);
            }
            else
            {
                if (taskDict.TryGetValue(task.ParentTaskId, out var parent))
                {
                    parent.Children.Add(task);
                }
                else
                {
                    rootTasks.Add(task);
                }
            }
        }
        
        return rootTasks;
    }

    /// <summary>
    /// Gets all tasks from a plan (flattened)
    /// </summary>
    /// <param name="plan">The plan to get tasks from</param>
    /// <returns>Flattened list of all tasks in the plan</returns>
    private List<TaskItem> GetAllTasksFromPlan(ProjectPlan plan)
    {
        var allTasks = new List<TaskItem>();
        foreach (var task in plan.Tasks)
        {
            allTasks.Add(task);
            AddChildTasksRecursively(task, allTasks);
        }
        return allTasks;
    }

    /// <summary>
    /// Recursively adds child tasks to the list
    /// </summary>
    /// <param name="parentTask">Parent task</param>
    /// <param name="allTasks">List to add tasks to</param>
    private void AddChildTasksRecursively(TaskItem parentTask, List<TaskItem> allTasks)
    {
        foreach (var child in parentTask.Children)
        {
            allTasks.Add(child);
            AddChildTasksRecursively(child, allTasks);
        }
    }

    /// <summary>
    /// Finds a task by ID within a plan's hierarchy
    /// </summary>
    /// <param name="plan">The plan to search in</param>
    /// <param name="taskId">ID of the task to find</param>
    /// <returns>Task if found, null otherwise</returns>
    private TaskItem? FindTaskInPlan(ProjectPlan plan, string taskId)
    {
        foreach (var task in plan.Tasks)
        {
            var found = FindTaskRecursively(task, taskId);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// Recursively searches for a task in the hierarchy
    /// </summary>
    /// <param name="currentTask">Current task to search</param>
    /// <param name="taskId">ID of the task to find</param>
    /// <returns>Task if found, null otherwise</returns>
    private TaskItem? FindTaskRecursively(TaskItem currentTask, string taskId)
    {
        if (currentTask.Id == taskId) return currentTask;
        
        foreach (var child in currentTask.Children)
        {
            var found = FindTaskRecursively(child, taskId);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// Updates a task in the database
    /// </summary>
    /// <param name="task">Updated task item</param>
    public async Task UpdateTaskAsync(TaskItem task)
    {
        var database = await GetDatabaseAsync();
        
        // Find the plan this task belongs to
        var plan = database.Plans.FirstOrDefault(p => p.Id == task.PlanId);
        if (plan == null)
        {
            throw new ArgumentException($"Plan with ID '{task.PlanId}' not found.");
        }

        // Find the existing task in the plan
        var existingTask = FindTaskInPlan(plan, task.Id);
        if (existingTask == null)
        {
            throw new ArgumentException($"Task with ID '{task.Id}' not found.");
        }

        // Update the task properties
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.AcceptanceCriteria = task.AcceptanceCriteria;
        existingTask.Status = task.Status;
        existingTask.Tags = new List<string>(task.Tags);
        existingTask.Groups = new List<string>(task.Groups);
        existingTask.EstimateHours = task.EstimateHours;
        existingTask.UpdatedAt = task.UpdatedAt;
        existingTask.CompletedAt = task.CompletedAt;

        await SaveDatabaseAsync(database);
    }
}
