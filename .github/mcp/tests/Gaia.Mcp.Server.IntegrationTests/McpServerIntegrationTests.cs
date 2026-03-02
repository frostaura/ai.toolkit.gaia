using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gaia.Mcp.Server.IntegrationTests;

/// <summary>
/// Custom factory that sets GAIA_DATA_DIR once before the server starts.
/// Shared across all tests via IClassFixture.
/// </summary>
public class GaiaMcpFactory : WebApplicationFactory<Program>
{
    public string DataDir { get; } = Path.Combine(Path.GetTempPath(), $"gaia-int-test-{Guid.NewGuid():N}");

    public GaiaMcpFactory()
    {
        Directory.CreateDirectory(DataDir);
        Environment.SetEnvironmentVariable("GAIA_DATA_DIR", DataDir);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (Directory.Exists(DataDir))
            try { Directory.Delete(DataDir, true); } catch { /* best-effort cleanup */ }
    }
}

public class McpServerIntegrationTests : IClassFixture<GaiaMcpFactory>
{
    private readonly HttpClient _client;
    private string? _sessionId;

    public McpServerIntegrationTests(GaiaMcpFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("Accept", "application/json, text/event-stream");
    }

    /// <summary>
    /// Parse SSE response body, extracting JSON from "data:" lines.
    /// MCP HTTP transport returns SSE format: event: message\ndata: {json}\n\n
    /// </summary>
    private static JsonElement ParseSseResponse(string body)
    {
        var lines = body.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.TrimEnd('\r');
            if (trimmed.StartsWith("data: "))
            {
                var json = trimmed["data: ".Length..];
                return JsonDocument.Parse(json).RootElement;
            }
        }
        return JsonDocument.Parse(body).RootElement;
    }

    private async Task EnsureSessionAsync()
    {
        if (_sessionId is not null) return;

        var initPayload = new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "initialize",
            @params = new
            {
                protocolVersion = "2025-03-26",
                capabilities = new { },
                clientInfo = new { name = "test-client", version = "1.0.0" }
            }
        };

        var response = await _client.PostAsJsonAsync("/mcp", initPayload);
        await response.Content.ReadAsStringAsync(); // consume body
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _sessionId = response.Headers.TryGetValues("Mcp-Session-Id", out var values)
            ? values.FirstOrDefault()
            : null;
        Assert.NotNull(_sessionId);
        _client.DefaultRequestHeaders.Add("Mcp-Session-Id", _sessionId);
    }

    private async Task<JsonElement> CallToolAsync(string tool, object arguments, int id = 2)
    {
        await EnsureSessionAsync();

        var content = JsonSerializer.Serialize(new
        {
            jsonrpc = "2.0",
            id,
            method = "tools/call",
            @params = new { name = tool, arguments }
        });

        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK,
            $"Tool '{tool}' returned {response.StatusCode}: {body[..Math.Min(500, body.Length)]}");

        return ParseSseResponse(body);
    }

    private static string GetToolResultText(JsonElement rpcResponse)
    {
        return rpcResponse
            .GetProperty("result")
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString()!;
    }

    [Fact]
    public async Task Server_Responds_To_Initialize()
    {
        await EnsureSessionAsync();
        Assert.NotNull(_sessionId);
    }

    [Fact]
    public async Task TaskLifecycle_CreateListUpdateMarkDone()
    {
        var createResult = await CallToolAsync("tasks_create", new
        {
            project = "int-task",
            title = "Integration test task"
        });

        var taskJson = JsonDocument.Parse(GetToolResultText(createResult)).RootElement;
        var taskId = taskJson.GetProperty("id").GetString()!;
        Assert.Equal(32, taskId.Length);

        var listResult = await CallToolAsync("tasks_list", new { project = "int-task" }, 3);
        var tasks = JsonDocument.Parse(GetToolResultText(listResult)).RootElement;
        Assert.True(tasks.GetArrayLength() >= 1);

        await CallToolAsync("tasks_update", new
        {
            project = "int-task",
            id = taskId,
            status = "doing"
        }, 4);

        var doneResult = await CallToolAsync("tasks_mark_done", new
        {
            project = "int-task",
            id = taskId,
            changedFiles = new[] { "src/test.cs" },
            testsAdded = new[] { "tests/test.cs" },
            manualRegressionLabels = new[] { "curl" }
        }, 5);

        var doneJson = JsonDocument.Parse(GetToolResultText(doneResult)).RootElement;
        Assert.True(doneJson.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task MemoryLifecycle_RememberRecallForgetClear()
    {
        await CallToolAsync("memory_remember", new
        {
            project = "int-mem",
            key = "build/cmd",
            value = "dotnet build"
        });

        var recallResult = await CallToolAsync("memory_recall", new
        {
            project = "int-mem"
        }, 3);
        var items = JsonDocument.Parse(GetToolResultText(recallResult)).RootElement;
        Assert.True(items.GetArrayLength() >= 1);

        await CallToolAsync("memory_forget", new
        {
            project = "int-mem",
            key = "build/cmd"
        }, 4);

        await CallToolAsync("memory_clear", new { project = "int-mem" }, 5);

        var finalRecall = await CallToolAsync("memory_recall", new
        {
            project = "int-mem"
        }, 6);
        var finalItems = JsonDocument.Parse(GetToolResultText(finalRecall)).RootElement;
        Assert.Equal(0, finalItems.GetArrayLength());
    }

    [Fact]
    public async Task SelfImproveLifecycle_LogListMarkAppliedClear()
    {
        var logResult = await CallToolAsync("self_improve_log", new
        {
            project = "int-si",
            suggestion = "Always check lint first",
            category = "ci"
        });

        var item = JsonDocument.Parse(GetToolResultText(logResult)).RootElement;
        var itemId = item.GetProperty("id").GetString()!;

        var listResult = await CallToolAsync("self_improve_list", new
        {
            project = "int-si"
        }, 3);
        var items = JsonDocument.Parse(GetToolResultText(listResult)).RootElement;
        Assert.True(items.GetArrayLength() >= 1);

        await CallToolAsync("self_improve_mark_applied", new { id = itemId }, 4);

        await CallToolAsync("self_improve_clear", new { project = "int-si" }, 5);
    }

    [Fact]
    public async Task ConcurrentTaskCreation_ProducesUniqueIds()
    {
        await EnsureSessionAsync();

        var createTasks = Enumerable.Range(0, 5).Select(async i =>
        {
            var result = await CallToolAsync("tasks_create", new
            {
                project = "int-concurrent",
                title = $"Task {i}"
            }, 100 + i);

            var taskJson = JsonDocument.Parse(GetToolResultText(result)).RootElement;
            return taskJson.GetProperty("id").GetString()!;
        }).ToArray();

        var ids = await Task.WhenAll(createTasks);
        Assert.Equal(ids.Length, ids.Distinct().Count());
    }
}
