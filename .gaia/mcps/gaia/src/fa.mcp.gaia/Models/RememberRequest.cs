using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Request DTO for remember operation
    /// </summary>
    [Description("Request to store a memory for later recall")]
    public class RememberRequest
    {
        [Description("Category grouping for the memory (e.g., issue, workaround, config, pattern, decision)")]
        public string Category { get; set; } = string.Empty;

        [Description("Unique key identifier within the category")]
        public string Key { get; set; } = string.Empty;

        [Description("The actual content/value to remember")]
        public string Value { get; set; } = string.Empty;
    }
}
