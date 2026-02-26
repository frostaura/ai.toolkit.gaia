using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Tools;

var builder = WebApplication.CreateBuilder(args);

var dataDir = Environment.GetEnvironmentVariable("GAIA_DATA_DIR")
    ?? Path.Combine(AppContext.BaseDirectory, "data");
var repoRoot = Environment.GetEnvironmentVariable("GAIA_REPO_ROOT")
    ?? Directory.GetCurrentDirectory();

var store = new JsonTaskStore(dataDir);
var tasksTool = new TasksTool(store, repoRoot);

var memoryStore = new ThreadSafeJsonStore<MemoryItem>(dataDir, ".memory.json");
var memoryTool = new MemoryTool(memoryStore);

var improvementStore = new ThreadSafeJsonStore<ImprovementItem>(dataDir, ".improvements.json");
var selfImproveTool = new SelfImproveTool(improvementStore);

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
    .WithTools(selfImproveTool);

var app = builder.Build();

app.MapMcp();

app.Run();
