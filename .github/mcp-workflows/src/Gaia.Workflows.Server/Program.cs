using Gaia.Workflows.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Suppress console logging — stdout is reserved for MCP stdio protocol
builder.Logging.AddFilter(_ => false);

var workflowsDir = Path.Combine(Directory.GetCurrentDirectory(), ".github", ".agaia-workflows");
var workflowsTool = new WorkflowsTool(workflowsDir);

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "gaia-workflows",
            Version = "1.0.0"
        };
    })
    .WithStdioServerTransport()
    .WithTools(workflowsTool);

var app = builder.Build();
await app.RunAsync();
