namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for agent delegation operations exposed as MCP server tools
/// </summary>
public interface IAgentsManager
{
    /// <summary>
    /// Delegates input to a named agent using fallback handling when direct delegation is not available.
    /// </summary>
    /// <param name="agentName">Name of the agent to delegate to</param>
    /// <param name="agentInput">Input payload for the agent (serialized as JSON string)</param>
    /// <returns>JSON string containing the result of delegation or a structured fallback response</returns>
    Task<string> DelegateToAgentFallbackAsync(string agentName, string agentInput);
}
