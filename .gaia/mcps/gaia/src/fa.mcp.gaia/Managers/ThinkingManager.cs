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
public class ThinkingManager : IThinkingManager
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
    public Task<string> DoubleCheckThoughtAsync(
        [Description("Current task context")] string taskContext,
        [Description("Current task acceptance criteria")] string taskAcceptanceContext,
        [Description("The thought or reasoning to critique")] string thought,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(thought))
            throw new ArgumentException("Thought cannot be null or empty.", nameof(thought));

        return Task.FromResult(@"
            You must now reflect on your thought and the context and improve your answer. Then continue.
        ");
    }
}
