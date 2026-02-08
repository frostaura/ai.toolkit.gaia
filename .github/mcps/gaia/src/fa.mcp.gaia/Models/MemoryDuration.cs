using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Defines how long a memory should persist
    /// </summary>
    public enum MemoryDuration
    {
        /// <summary>
        /// Memory exists only for the current session (lost when service restarts)
        /// </summary>
        [Description("Memory persists only for the current terminal session")]
        SessionLength = 0,

        /// <summary>
        /// Memory is permanently stored on disk and survives restarts
        /// </summary>
        [Description("Memory is permanently stored and persists across sessions")]
        ProjectWide = 1
    }
}
