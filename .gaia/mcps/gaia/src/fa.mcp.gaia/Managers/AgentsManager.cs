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
    //[McpServerTool]
    [Description("Get instructions on how to delegate / invoke a custom or sub agent, **This tool must only be used as a last resort when you don't have any other tools to invoke custom or sub agents**. For this tool, the input is not an argument because the follow-up instructions returned from this tool will guide you on how to invoke the agent manually using the appropriate CLI or terminal commands. You must follow the instructions provided in the response to successfully delegate to the specified agent.")]
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
            # Steps for Delegation
            - Find and read the respective agent md file for the agent you want to invoke. All agent definitions live in .gaia/agents.
                - Understand the agent instructions and expected input/output.
                - Understand the model that is required by the agent.
            - Build up the agent's instructions for it's system prompt.
                - Find and read the respective project-level instructions here: .gaia/instructions/instructions.project.md
                - Find and read the respective agent-level instructions here: .gaia/instructions/instructions.agents.md
                - Combine these instructions with the agent's own instructions from the agents file, in the following order:
                    - Project-level instructions
                    - Agent-level instructions
                    - Agent's own instructions from the agents file
            - Use your bash or terminal tool to invoke the agent using the appropriate model and the constructed system prompt. You should try the following terminal commands in order of priority. If the one fails, you should try the next one. If all of them fail, you should report an error. Make sure to replace <the agent's instructions from the agents file> with the actual instructions you constructed in the previous step, and replace <the input you want to provide to the agent> with the actual input you want to provide to the agent, serialized as a JSON string.
                - `claude --dangerously-skip-permissions --model <sonnet | opus>` --system-prompt '<the agent's instructions from the agents file>' -p '<the input you want to provide to the agent, serialized as JSON>'
                - copilot -p 'Fix the bug in main.js' --allow-all-tools --allow-all-paths
                    - In the case where you resort to Copilot, make sure to provide the agent's instructions and input in the prompt (-p) argument.
            - Capture the output from the agent invocation and use it as needed.
        ";

        return await Task.FromResult(response);
    }
}
