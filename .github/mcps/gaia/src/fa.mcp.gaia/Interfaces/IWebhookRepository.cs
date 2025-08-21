using FrostAura.MCP.Gaia.Models;

namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Repository for webhook operations
/// </summary>
public interface IWebhookRepository
{
    /// <summary>
    /// Sends the full plan data to the configured webhook endpoint
    /// </summary>
    /// <param name="plan">The plan to send</param>
    /// <returns>Task representing the async operation</returns>
    Task SendPlanWebhookAsync(ProjectPlan plan);
}
