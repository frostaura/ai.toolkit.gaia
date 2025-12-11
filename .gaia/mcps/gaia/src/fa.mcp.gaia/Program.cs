using FrostAura.MCP.Gaia.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Core MCP server - Essential tools for in-memory task and memory management
var builder = Host.CreateApplicationBuilder(args);

// Configure logging - MCP uses stdio for communication, so logs go to stderr
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    // All logs go to stderr to avoid interfering with MCP stdio communication
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add configuration
builder.Configuration
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Application:Name"] = "fa.mcp.gaia",
        ["Application:Version"] = "2.0.0",
        // Set log levels - FrostAura namespace gets detailed logging
        ["Logging:LogLevel:Default"] = "Warning",
        ["Logging:LogLevel:FrostAura.MCP.Gaia"] = "Information",
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
