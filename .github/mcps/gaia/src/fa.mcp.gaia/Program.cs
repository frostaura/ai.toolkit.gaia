using FrostAura.MCP.Gaia.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Determine transport mode: "http" for remote/container deployment, "stdio" for local (default)
var transport = Environment.GetEnvironmentVariable("MCP_TRANSPORT") ?? "stdio";
var isHttpMode = transport.Equals("http", StringComparison.OrdinalIgnoreCase);

if (isHttpMode)
{
    // HTTP/SSE transport - used when deployed as a remote MCP server in a container
    var builder = WebApplication.CreateBuilder(args);

    RegisterManagers(builder.Services);

    // Configure MCP Server with HTTP transport
    builder.Services
        .AddMcpServer()
        .WithHttpTransport()
        .WithToolsFromAssembly();

    var app = builder.Build();

    app.MapMcp("/mcp");

    await app.RunAsync();
}
else
{
    // STDIO transport - used when running locally via mcp-config.json
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

    RegisterManagers(builder.Services);

    // Configure MCP Server
    builder.Services
        .AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();

    var host = builder.Build();

    // Start the host directly - no database migration needed
    await host.RunAsync();
}

static void RegisterManagers(IServiceCollection services)
{
    services.AddSingleton<TaskManager>();
    services.AddSingleton<MemoryManager>();
    services.AddSingleton<ImprovementManager>();
}
