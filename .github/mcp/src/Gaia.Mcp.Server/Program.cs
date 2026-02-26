var builder = WebApplication.CreateBuilder(args);

var dataDir = Environment.GetEnvironmentVariable("GAIA_DATA_DIR")
    ?? Path.Combine(AppContext.BaseDirectory, "data");
var repoRoot = Environment.GetEnvironmentVariable("GAIA_REPO_ROOT")
    ?? Directory.GetCurrentDirectory();

var store = new JsonTaskStore(dataDir);
var tasksTool = new TasksTool(store, repoRoot);

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
    .WithTools(tasksTool);

var app = builder.Build();

app.MapMcp();

app.Run();
