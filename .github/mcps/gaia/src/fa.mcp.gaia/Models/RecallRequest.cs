using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Request DTO for recall operation
    /// </summary>
    [Description("Request to search for memories")]
    public class RecallRequest
    {
        [Description("Query to search for in memories (supports fuzzy search across category, key, and value)")]
        public string Query { get; set; } = string.Empty;

        [Description("Maximum number of results to return (default: 20)")]
        public int MaxResults { get; set; } = 20;
    }
}
