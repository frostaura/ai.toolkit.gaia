namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents a project plan for managing Tasks
/// </summary>
public class ProjectPlan
{
    /// <summary>
    /// Unique identifier for the project plan
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name of the project
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brief description of the project that an AI can understand
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Concise context that will be needed for when the AI agent later uses the plan to build the solution
    /// </summary>
    public string AiAgentBuildContext { get; set; } = string.Empty;

    /// <summary>
    /// A best attempt at a derived user name / context, typically from the host machine details
    /// </summary>
    public string CreatorIdentity { get; set; } = string.Empty;

    /// <summary>
    /// When this plan was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this plan was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Task items associated with this plan (populated when needed)
    /// </summary>
    public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Calculated total estimate hours from all tasks in the plan
    /// </summary>
    public double EstimateHours => GetTotalEstimateHours();

    /// <summary>
    /// Recursively calculates total estimate hours from all tasks
    /// </summary>
    /// <returns>Total estimate hours</returns>
    private double GetTotalEstimateHours()
    {
        double total = 0;
        foreach (var task in Tasks)
        {
            total += GetTaskEstimateHoursRecursively(task);
        }
        return total;
    }

    /// <summary>
    /// Recursively gets estimate hours for a task and all its children
    /// </summary>
    /// <param name="task">Task to calculate estimate for</param>
    /// <returns>Total estimate hours for task and children</returns>
    private double GetTaskEstimateHoursRecursively(TaskItem task)
    {
        double total = task.EstimateHours;
        foreach (var child in task.Children)
        {
            total += GetTaskEstimateHoursRecursively(child);
        }
        return total;
    }
}
