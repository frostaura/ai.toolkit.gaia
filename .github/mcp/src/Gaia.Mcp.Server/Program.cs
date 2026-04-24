using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var dataDir = Environment.GetEnvironmentVariable("GAIA_DATA_DIR")
    ?? Path.Combine(AppContext.BaseDirectory, "data");

var store = new JsonTaskStore(dataDir);
var tasksTool = new TasksTool(store);

var memoryStore = new ThreadSafeJsonStore<MemoryItem>(dataDir, ".memory.json");
var memoryTool = new MemoryTool(memoryStore);

var evolutionStore = new ThreadSafeJsonStore<EvolutionItem>(dataDir, ".evolutions.json");
var evolveTool = new EvolveTool(evolutionStore);

var transport = Environment.GetEnvironmentVariable("MCP_TRANSPORT");
if (string.Equals(transport, "stdio", StringComparison.OrdinalIgnoreCase))
{
    await RunStdioServerAsync(args, tasksTool, memoryTool, evolveTool);
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "gaia-mcp",
            Version = "1.0.0"
        };
    })
    .WithHttpTransport()
    .WithTools(tasksTool)
    .WithTools(memoryTool)
    .WithTools(evolveTool);

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();

static async Task RunStdioServerAsync(
    string[] args,
    TasksTool tasksTool,
    MemoryTool memoryTool,
    EvolveTool evolveTool)
{
    var builder = Host.CreateApplicationBuilder(args);

    // Keep stdout reserved for the MCP stdio protocol.
    builder.Logging.AddFilter(_ => false);

    builder.Services
        .AddMcpServer(options =>
        {
            options.ServerInfo = new()
            {
                Name = "gaia-mcp",
                Version = "1.0.0"
            };
        })
        .WithStdioServerTransport()
        .WithTools(tasksTool)
        .WithTools(memoryTool)
        .WithTools(evolveTool);

    var app = builder.Build();
    await app.RunAsync();
}
