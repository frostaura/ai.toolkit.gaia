using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response DTO for remember operation
    /// </summary>
    [Description("Response confirming a memory was stored or updated")]
    public class RememberResponse
    {
        [Description("Whether the operation was successful")]
        public bool Success { get; set; }

        [Description("Human-readable message describing what happened")]
        public string Message { get; set; } = string.Empty;

        [Description("Whether this was an update to an existing memory (true) or a new memory (false)")]
        public bool WasUpdate { get; set; }

        [Description("The memory that was stored or updated")]
        public GaiaMemory? Memory { get; set; }
    }
}
