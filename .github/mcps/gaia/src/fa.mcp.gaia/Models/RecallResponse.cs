using System.Collections.Generic;
using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response DTO for recall operation
    /// </summary>
    [Description("Response containing memories matching a search query")]
    public class RecallResponse
    {
        [Description("Number of results returned (may be limited by maxResults)")]
        public int Count { get; set; }

        [Description("Total number of memories that matched the query before limiting")]
        public int TotalMatches { get; set; }

        [Description("The search query that was used")]
        public string Query { get; set; } = string.Empty;

        [Description("The search mode used: 'fuzzy' for fuzzy matching")]
        public string SearchMode { get; set; } = "fuzzy";

        [Description("Optional message (e.g., when no results found)")]
        public string? Message { get; set; }

        [Description("List of memories matching the query, ordered by relevance")]
        public List<MemorySearchResult> Results { get; set; } = new();
    }
}
