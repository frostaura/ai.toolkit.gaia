using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response after logging an improvement
    /// </summary>
    [Description("Response confirming the improvement was logged")]
    public class LogImprovementResponse
    {
        [Description("Whether the operation was successful")]
        public bool Success { get; set; }

        [Description("Human-readable message about the operation")]
        public string Message { get; set; } = string.Empty;

        [Description("The improvement that was logged")]
        public GaiaImprovement? Improvement { get; set; }
    }
}
