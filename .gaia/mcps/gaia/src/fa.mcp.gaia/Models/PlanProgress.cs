namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents progress information for a project plan
/// </summary>
public class PlanProgress
{
    /// <summary>
    /// ID of the project plan
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the project plan
    /// </summary>
    public string PlanName { get; set; } = string.Empty;

    /// <summary>
    /// Total number of Tasks in the plan
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of completed Tasks
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Number of Tasks currently in progress
    /// </summary>
    public int InProgressTasks { get; set; }

    /// <summary>
    /// Number of pending Tasks (not started)
    /// </summary>
    public int PendingTasks { get; set; }

    /// <summary>
    /// Completion percentage (0-100)
    /// </summary>
    public double CompletionPercentage { get; set; }

    /// <summary>
    /// Total estimated hours for the project
    /// </summary>
    public double ProjectEstimateHours { get; set; }

    /// <summary>
    /// Total estimated hours from all Tasks combined
    /// </summary>
    public double TotalTaskEstimateHours { get; set; }

    /// <summary>
    /// Estimated hours from completed Tasks
    /// </summary>
    public double CompletedTaskEstimateHours { get; set; }

    /// <summary>
    /// Estimated hours from Tasks in progress
    /// </summary>
    public double InProgressTaskEstimateHours { get; set; }

    /// <summary>
    /// Estimated hours from pending Tasks
    /// </summary>
    public double PendingTaskEstimateHours { get; set; }

    /// <summary>
    /// Completion percentage based on estimated hours (0-100)
    /// </summary>
    public double EstimateCompletionPercentage { get; set; }
}
