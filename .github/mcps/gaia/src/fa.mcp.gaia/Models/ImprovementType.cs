using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Type of improvement request
    /// </summary>
    [Description("Type of improvement request logged by agents")]
    public enum ImprovementType
    {
        [Description("A pain point or frustration encounter during work")]
        PainPoint,

        [Description("A missing tool or capability that would help")]
        MissingCapability,

        [Description("A suggestion for a better workflow or process")]
        WorkflowImprovement,

        [Description("An area where more context or guidance is needed")]
        KnowledgeGap,

        [Description("A general enhancement or feature request")]
        Enhancement
    }
}
