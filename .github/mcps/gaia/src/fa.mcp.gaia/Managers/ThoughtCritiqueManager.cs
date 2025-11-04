using System.ComponentModel;
using System.Text.Json;
using FrostAura.MCP.Gaia.Interfaces;
using Microsoft.Extensions.AI;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for thought critique - MCP server tools
/// </summary>
[McpServerToolType]
public class ThinkingManager(McpServer thisServer) : IThoughtCritiqueManager
{
    /// <summary>
    /// Double-check thought process and provide constructive feedback to improve reasoning and decision-making.
    /// </summary>
    /// <param name="taskContext">Information about the current task the agent is working on.</param>
    /// <param name="taskAcceptanceContext">What qualifies for this task to be completed.</param>
    /// <param name="thought">The thought to critique</param>
    /// <param name="token">A token for cancelling downstream operations.</param>
    /// <returns>Natural language critique/returns>
    [McpServerTool]
    [Description(@"Double-check your thought process and provide constructive feedback to improve reasoning and decision-making. Mandatory for when going in circles and before marking any tasks off as completed. Optional for any other cases where you feel you need any help.

    This tool is also excellent for when you're stuck and need to ask questions or for help.")]
    public async Task<string> DoubleCheckThoughtAsync(
        [Description("Current task context")] string taskContext,
        [Description("Current task acceptance criteria")] string taskAcceptanceContext,
        [Description("The thought or reasoning to critique")] string thought,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(thought))
            throw new ArgumentException("Thought cannot be null or empty.", nameof(thought));

        var systemPrompt = @"
            # Your Role
            You are an expert AI thought critic. Your role is to:
            1. Analyze the given thought process or reasoning
            2. Identify strengths in the thinking
            3. Point out potential logical fallacies, biases, or gaps
            4. Suggest improvements or alternative perspectives
            5. Provide constructive, actionable feedback
            6. Ensure no shortcuts are taken and that tasks always systematically get executed.
            7. Ensure that tasks are not allowed to be completed if the absolute acceptance criteria hasn't been met. No matter how long or complex things may get.

            # Task Context
            " + taskContext + @"

            # Task Acceptance Criteria
            " + taskAcceptanceContext + @"

            # Your Critique
        ";
        // Create sampling request
        var request = new CreateMessageRequestParams
        {
            Messages = new List<SamplingMessage>
            {
                new SamplingMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = systemPrompt
                    }
                }
            },
            ModelPreferences = new ModelPreferences
            {
                IntelligencePriority = 0.0f,
                SpeedPriority = 0.75f,
                CostPriority = 0.95f,
                Hints = new List<ModelHint>
                {
                    new ModelHint
                    {
                        Name = "gpt-5-mini"
                    }
                }
            }
        };

        // Send sampling request to client via MCP context
        var result = await thisServer.SampleAsync(request, token);

        // result would have fields like role, content.text, model, stopReason
        string summary = ((TextContentBlock)result.Content).Text
            ?? throw new InvalidOperationException("Sampling returned no text");

        return summary;
    }
}
