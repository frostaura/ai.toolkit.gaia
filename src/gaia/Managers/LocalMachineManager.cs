using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Configuration;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for local machine operations and information retrieval with integrated MCP tools
/// </summary>
[McpServerToolType]
public class LocalMachineManager : ILocalMachineManager
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the LocalMachineManager
    /// </summary>
    /// <param name="configuration">Configuration for any settings</param>
    public LocalMachineManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the current host machine name via MCP
    /// </summary>
    /// <returns>JSON string containing the host machine name and additional system information</returns>
    [McpServerTool]
    [Description("Gets the current host machine name and basic system information. Useful for identifying the local environment and system context.")]
    public Task<string> GetHostMachineNameAsync()
    {
        var hostInfo = new
        {
            machineName = Environment.MachineName,
            userName = Environment.UserName,
            osVersion = Environment.OSVersion.ToString(),
            platform = Environment.OSVersion.Platform.ToString(),
            processorCount = Environment.ProcessorCount,
            workingDirectory = Environment.CurrentDirectory,
            dotNetVersion = Environment.Version.ToString(),
            is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
            is64BitProcess = Environment.Is64BitProcess,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(hostInfo, JsonConfiguration.GetApiOptions());

        return Task.FromResult(json);
    }
}
