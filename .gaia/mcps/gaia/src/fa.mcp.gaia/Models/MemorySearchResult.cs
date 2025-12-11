using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// A single memory search result with relevance score
    /// </summary>
    [Description("A memory search result with its relevance score")]
    public class MemorySearchResult
    {
        [Description("The memory that matched the search query")]
        public GaiaMemory Memory { get; set; } = new();

        [Description("Relevance score from 0-100 indicating how well the memory matched the query")]
        public double Relevance { get; set; }
    }
}
