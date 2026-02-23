using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Request to log an improvement opportunity
    /// </summary>
    [Description("Request to log an improvement, frustration, or enhancement opportunity")]
    public class LogImprovementRequest
    {
        [Description("Type of improvement: PainPoint, MissingCapability, WorkflowImprovement, KnowledgeGap, Enhancement")]
        public ImprovementType Type { get; set; } = ImprovementType.Enhancement;

        [Description("The agent logging this improvement (e.g., architect, developer, analyst, tester)")]
        public string Agent { get; set; } = string.Empty;

        [Description("The project where this improvement was identified (optional, for cross-project context)")]
        public string? ProjectName { get; set; }

        [Description("Brief title/summary of the improvement (2-10 words)")]
        public string Title { get; set; } = string.Empty;

        [Description("Detailed description of the issue, frustration, or opportunity")]
        public string Description { get; set; } = string.Empty;

        [Description("What was the agent trying to accomplish when this issue was encountered (optional)")]
        public string? Context { get; set; }

        [Description("Specific suggestions on how to address this improvement (optional)")]
        public string? Suggestion { get; set; }

        [Description("Priority level: Low, Medium, High, Critical (default: Medium)")]
        public string Priority { get; set; } = "Medium";
    }
}
