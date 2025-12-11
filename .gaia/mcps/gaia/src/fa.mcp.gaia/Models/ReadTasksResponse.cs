using System.Collections.Generic;
using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response DTO for read_tasks operation
    /// </summary>
    [Description("Response containing a list of tasks with summary information")]
    public class ReadTasksResponse
    {
        [Description("Human-readable summary of the task count and filter applied")]
        public string Summary { get; set; } = string.Empty;

        [Description("The filter that was applied: 'active only' or 'all tasks'")]
        public string Filter { get; set; } = string.Empty;

        [Description("Total number of tasks returned")]
        public int Count { get; set; }

        [Description("List of tasks matching the filter criteria")]
        public List<GaiaTask> Tasks { get; set; } = new();
    }
}
