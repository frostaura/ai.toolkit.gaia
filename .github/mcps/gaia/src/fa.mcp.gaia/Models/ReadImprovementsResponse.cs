using System.Collections.Generic;
using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Response containing filtered improvements
    /// </summary>
    [Description("Response containing a list of improvements with filtering applied")]
    public class ReadImprovementsResponse
    {
        [Description("Summary of the results")]
        public string Summary { get; set; } = string.Empty;

        [Description("Description of the filter applied")]
        public string Filter { get; set; } = string.Empty;

        [Description("Number of improvements returned")]
        public int Count { get; set; }

        [Description("List of improvements")]
        public List<GaiaImprovement> Improvements { get; set; } = new();
    }
}
