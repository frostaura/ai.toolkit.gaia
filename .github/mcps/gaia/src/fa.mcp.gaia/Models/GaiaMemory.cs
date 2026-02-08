using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Represents a memory entry with consistent structure
    /// </summary>
    [Description("A memory entry storing knowledge, decisions, or context for future recall")]
    public class GaiaMemory
    {
        [Description("Category grouping for the memory (e.g., issue, workaround, config, pattern, decision)")]
        public string Category { get; set; } = string.Empty;

        [Description("Unique key identifier within the category")]
        public string Key { get; set; } = string.Empty;

        [Description("The actual content/value being remembered")]
        public string Value { get; set; } = string.Empty;

        [Description("Duration of memory persistence")]
        public MemoryDuration Duration { get; set; } = MemoryDuration.SessionLength;

        [Description("UTC timestamp when the memory was first created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Description("UTC timestamp when the memory was last modified")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Composite key for dictionary storage (category/key)
        /// </summary>
        [JsonIgnore]
        public string CompositeKey => $"{Category}/{Key}".ToLowerInvariant();
    }
}
