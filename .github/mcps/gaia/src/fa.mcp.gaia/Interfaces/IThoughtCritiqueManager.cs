namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for thought critique - MCP server tools
/// </summary>
public interface IThoughtCritiqueManager
{
    /// <summary>
    /// Critique thought.
    /// </summary>
    /// <param name="taskContext">Information about the current task the agent is working on.</param>
    /// <param name="taskAcceptanceContext">What qualifies for this task to be completed.</param>
    /// <param name="thought">The thought to critique</param>
    /// <param name="token">A token for cancelling downstream operations.</param>
    /// <returns>Natural language critique/returns>
    Task<string> DoubleCheckThoughtAsync(string taskContext, string taskAcceptanceContext, string thought, CancellationToken token);
}
