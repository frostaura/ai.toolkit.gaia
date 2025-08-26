using FrostAura.MCP.Gaia.Configuration;
using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// Repository for webhook operations
/// </summary>
public class WebhookRepository : IWebhookRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WebhookRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public WebhookRepository(HttpClient httpClient, IConfiguration configuration, ILogger<WebhookRepository> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _jsonOptions = JsonConfiguration.GetApiOptions();
    }

    /// <summary>
    /// Sends the full plan data to the configured webhook endpoint
    /// </summary>
    /// <param name="plan">The plan to send</param>
    /// <returns>Task representing the async operation</returns>
    public async Task SendPlanWebhookAsync(ProjectPlan plan)
    {
        try
        {
            var webhookUrl = _configuration["TaskPlanner:WebhookUrl"];
            
            // Skip if no webhook URL is configured
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                _logger.LogDebug("No webhook URL configured, skipping webhook call");
                return;
            }

            // Serialize the plan to JSON
            var jsonPayload = JsonSerializer.Serialize(plan, _jsonOptions);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Add headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "FrostAura.MCP.Gaia/1.0.0");

            _logger.LogDebug("Sending plan webhook to {WebhookUrl} for plan {PlanId}", webhookUrl, plan.Id);

            // Send the POST request
            var response = await _httpClient.PostAsync(webhookUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Successfully sent plan webhook for plan {PlanId}", plan.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send plan webhook for plan {PlanId}. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    plan.Id, response.StatusCode, response.ReasonPhrase);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending plan webhook for plan {PlanId}", plan.Id);
            // Don't rethrow - webhook failures shouldn't break the main functionality
        }
    }
}
