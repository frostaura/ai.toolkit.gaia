using FrostAura.MCP.Gaia.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Core MCP server - Essential tools using JSONL for tasks and memory
var builder = Host.CreateApplicationBuilder(args);

// Configure minimal logging for MCP compliance
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Warning;
});

// Add configuration
builder.Configuration
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Application:Name"] = "fa.mcp.gaia",
        ["Application:Version"] = "2.0.0",
        ["Logging:LogLevel:Default"] = "Warning",
        ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Warning",
        ["Logging:LogLevel:ModelContextProtocol"] = "Warning"
    });

// Register core manager
builder.Services.AddScoped<CoreManager>();

// Configure MCP Server
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var host = builder.Build();

// Start the host directly - no database migration needed
await host.RunAsync();
