using FrostAura.MCP.Gaia;
using FrostAura.MCP.Gaia.Data;
using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Normal MCP server execution
var builder = Host.CreateApplicationBuilder(args);

// Configure logging to use stderr for MCP compliance
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add configuration - load from appsettings.json and add embedded defaults
builder.Configuration
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Application:Name"] = "fa.mcp.gaia",
        ["Application:Version"] = "1.0.1",
        ["Logging:LogLevel:Default"] = "Warning",
        ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Warning",
        ["Logging:LogLevel:ModelContextProtocol"] = "Warning",

        ["TaskPlanner:WebhookUrl"] = "http://localhost:5001/api/webhook"
    });

// Register Entity Framework
builder.Services.AddDbContext<TaskPlannerDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                          "Data Source=.gaia/state.db";
    options.UseSqlite(connectionString);
});

// Register Task Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITaskPlannerRepository, TaskPlannerRepository>();
builder.Services.AddScoped<IWebhookRepository, WebhookRepository>();

// Register Memory Services
builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();

// Register Managers (now includes MCP tools)
builder.Services.AddScoped<ITaskPlannerManager, TaskPlannerManager>();
builder.Services.AddScoped<ILocalMachineManager, LocalMachineManager>();
builder.Services.AddScoped<IMemoryManager, MemoryManager>();
builder.Services.AddScoped<IAgentsManager, AgentsManager>();

// Configure MCP Server
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var host = builder.Build();

// Ensure database is created and up to date
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskPlannerDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Start the host directly
await host.RunAsync();
