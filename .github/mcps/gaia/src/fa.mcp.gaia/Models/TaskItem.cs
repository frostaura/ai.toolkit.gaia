using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents a Task item with support for nested Tasks
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Unique identifier for the Task item
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ID of the project plan this Task belongs to
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// ID of the parent Task if this is a nested Task
    /// </summary>
    public string? ParentTaskId { get; set; }

    /// <summary>
    /// Title/description of the Task that an AI can understand
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description with acceptance criteria
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Specific acceptance criteria for this Task
    /// </summary>
    public string AcceptanceCriteria { get; set; } = string.Empty;

    /// <summary>
    /// Status of the Task
    /// </summary>
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;

    /// <summary>
    /// Tags for grouping and categorization
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Collection of group names this Task belongs to (e.g., "Release 1.0", "Backend", "Frontend")
    /// </summary>
    public List<string> Groups { get; set; } = new List<string>();

    /// <summary>
    /// Estimated hours for completing this Task item
    /// </summary>
    public double EstimateHours { get; set; } = 0.0;

    /// <summary>
    /// When this Task was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this Task was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this Task was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Child Task items nested under this Task
    /// </summary>
    public List<TaskItem> Children { get; set; } = new List<TaskItem>();
}
