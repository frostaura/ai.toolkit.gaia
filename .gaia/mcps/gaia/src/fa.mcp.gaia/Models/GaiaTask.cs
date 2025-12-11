using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Represents a task in the system with consistent structure
    /// </summary>
    [Description("A task representing a unit of work to be completed")]
    public class GaiaTask
    {
        [Description("Unique identifier for the task (e.g., E-1/S-1/F-1/T-1 for hierarchical WBS)")]
        public string Id { get; set; } = string.Empty;

        [Description("Detailed description of what the task involves")]
        public string Description { get; set; } = string.Empty;

        [Description("Current status of the task: Pending, InProgress, Completed, Blocked, or Cancelled")]
        public GaiaTaskStatus Status { get; set; } = GaiaTaskStatus.Pending;

        [Description("The agent or person assigned to complete this task (optional)")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AssignedTo { get; set; }

        [Description("UTC timestamp when the task was first created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Description("UTC timestamp when the task was last modified")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
