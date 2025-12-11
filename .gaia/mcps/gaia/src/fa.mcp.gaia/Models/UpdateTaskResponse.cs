using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response DTO for update_task operation
    /// </summary>
    [Description("Response confirming a task update or creation")]
    public class UpdateTaskResponse
    {
        [Description("Whether the operation was successful")]
        public bool Success { get; set; }

        [Description("Human-readable message describing what happened")]
        public string Message { get; set; } = string.Empty;

        [Description("The task that was created or updated")]
        public GaiaTask? Task { get; set; }
    }
}
