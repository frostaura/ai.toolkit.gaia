using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Represents an improvement request logged by an agent
    /// </summary>
    [Description("An improvement request capturing agent frustrations and enhancement opportunities")]
    public class GaiaImprovement
    {
        [Description("Unique identifier for the improvement request")]
        public string Id { get; set; } = string.Empty;

        [Description("Type of improvement being requested")]
        public ImprovementType Type { get; set; } = ImprovementType.Enhancement;

        [Description("The agent that logged this improvement request")]
        public string Agent { get; set; } = string.Empty;

        [Description("The project where this improvement was identified (optional, for context)")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ProjectName { get; set; }

        [Description("Brief title/summary of the improvement")]
        public string Title { get; set; } = string.Empty;

        [Description("Detailed description of the issue or opportunity")]
        public string Description { get; set; } = string.Empty;

        [Description("What was the agent trying to accomplish when this issue was encountered")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Context { get; set; }

        [Description("Specific suggestions on how to address this improvement")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Suggestion { get; set; }

        [Description("Priority level: Low, Medium, High, Critical")]
        public string Priority { get; set; } = "Medium";

        [Description("Current status: Logged, UnderReview, Planned, Implemented, Dismissed")]
        public string Status { get; set; } = "Logged";

        [Description("UTC timestamp when the improvement was first logged")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Description("UTC timestamp when the improvement was last modified")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
