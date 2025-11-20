using System.ComponentModel;
using System.Text.Json;
using FrostAura.MCP.Gaia.Configuration;
using FrostAura.MCP.Gaia.Interfaces;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager responsible for delegating work to named agents and providing a safe fallback.
/// </summary>
[McpServerToolType]
public class AgentsManager : IAgentsManager
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<AgentsManager>? _logger;

    public AgentsManager(ILogger<AgentsManager>? logger = null)
    {
        _jsonOptions = JsonConfiguration.GetApiOptions();
        _logger = logger;
    }

    /// <summary>
    /// Delegates the provided input to the specified agent. If direct delegation is not available,
    /// returns a structured fallback response containing the provided input and agent name.
    /// </summary>
    /// <param name="agentName">Target agent name</param>
    /// <param name="agentInput">Input payload for the agent (serialized JSON)</param>
    /// <returns>JSON string containing delegation result or fallback response</returns>
    [McpServerTool]
    [Description("Get instructions on how to delegate / invoke a custom/sub agent, **This tool must only be used as a last resort when you don't have any other tools to invoke custom/sub agents**.")]
    public async Task<string> DelegateToAgentFallbackAsync(
        [Description("Name of the agent to delegate to")] string agentName,
        [Description("Input payload for the agent (serialized as JSON string)")] string agentInput)
    {
        _logger?.LogInformation("=== DelegateToAgentFallbackAsync ===");
        _logger?.LogInformation("AgentName: '{AgentName}'", agentName);
        _logger?.LogInformation("AgentInput: {AgentInput}", agentInput);

        if (string.IsNullOrWhiteSpace(agentName))
            throw new ArgumentException("Agent name cannot be null or empty.", nameof(agentName));
        if (agentInput == null)
            throw new ArgumentNullException(nameof(agentInput));

        var response = @"
            This tool is a fallback mechanism for delegating to custom/sub agents when no direct delegation is available.

            Here are your instructions to follow:
            - Acknowledge that direct delegation to the specified agent is not possible.
            - Yield a structured response containing
                - The input with which to invoke the agent with.
                - A contination brief protocol that describes how you should be called in the future, with the agent's response, to resume your process.

            Rules:
            - Never just propagate a delegation request from another agent without processing it yourself, if you have the tools to.
        ";

        return await Task.FromResult(response);
    }
}
