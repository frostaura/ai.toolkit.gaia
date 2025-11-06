using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ID of the project plan this Task belongs to
    /// </summary>
    [Required]
    [MaxLength(36)]
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// ID of the parent Task if this is a nested Task
    /// </summary>
    [MaxLength(36)]
    public string? ParentTaskId { get; set; }

    /// <summary>
    /// Title/description of the Task that an AI can understand
    /// </summary>
    [Required]
    [MaxLength(500)]
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
    /// Tags for grouping and categorization - stored as JSON
    /// </summary>
    [NotMapped]
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Tags serialized as JSON for database storage
    /// </summary>
    [Column("Tags")]
    public string TagsJson
    {
        get => JsonSerializer.Serialize(Tags);
        set => Tags = string.IsNullOrEmpty(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
    }

    /// <summary>
    /// Collection of group names this Task belongs to (e.g., "Release 1.0", "Backend", "Frontend") - stored as JSON
    /// </summary>
    [NotMapped]
    public List<string> Groups { get; set; } = new List<string>();

    /// <summary>
    /// Groups serialized as JSON for database storage
    /// </summary>
    [Column("Groups")]
    public string GroupsJson
    {
        get => JsonSerializer.Serialize(Groups);
        set => Groups = string.IsNullOrEmpty(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
    }

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
    [NotMapped]
    public List<TaskItem> Children { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Navigation property to the parent task
    /// </summary>
    [JsonIgnore]
    public virtual TaskItem? ParentTask { get; set; }

    /// <summary>
    /// Navigation property to child tasks
    /// </summary>
    [JsonIgnore]
    public virtual List<TaskItem> ChildTasks { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Navigation property to the project plan
    /// </summary>
    [JsonIgnore]
    public virtual ProjectPlan? Plan { get; set; }
}
