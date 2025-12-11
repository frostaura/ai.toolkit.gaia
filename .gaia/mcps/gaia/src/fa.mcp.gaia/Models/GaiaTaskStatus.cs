using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Valid task statuses
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GaiaTaskStatus
    {
        [Description("Task has not been started yet")]
        Pending,
        [Description("Task is currently being worked on")]
        InProgress,
        [Description("Task has been completed successfully")]
        Completed,
        [Description("Task is blocked by an external dependency or issue")]
        Blocked,
        [Description("Task has been cancelled and will not be completed")]
        Cancelled
    }
}
