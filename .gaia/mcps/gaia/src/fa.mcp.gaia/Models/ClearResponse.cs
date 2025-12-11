using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response DTO for clear operations
    /// </summary>
    [Description("Response confirming items were cleared from memory")]
    public class ClearResponse
    {
        [Description("Whether the operation was successful")]
        public bool Success { get; set; }

        [Description("Human-readable message describing what happened")]
        public string Message { get; set; } = string.Empty;

        [Description("Number of items that were cleared")]
        public int ClearedCount { get; set; }
    }
}
