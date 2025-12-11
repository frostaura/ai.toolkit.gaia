using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Request DTO for update_task operation
    /// </summary>
    [Description("Request to create or update a task")]
    public class UpdateTaskRequest
    {
        [Description("Unique identifier for the task (e.g., E-1/S-1/F-1/T-1 for hierarchical WBS)")]
        public string TaskId { get; set; } = string.Empty;

        [Description("Detailed description of what the task involves")]
        public string Description { get; set; } = string.Empty;

        [Description("Current status of the task: Pending, InProgress, Completed, Blocked, or Cancelled")]
        public GaiaTaskStatus Status { get; set; } = GaiaTaskStatus.Pending;

        [Description("The agent or person assigned to complete this task (optional)")]
        public string? AssignedTo { get; set; }
    }
}
