using System.Text.Json;
using FrostAura.MCP.Gaia.Converters;
using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Configuration;

/// <summary>
/// Provides centralized JSON serialization configuration for the application
/// </summary>
public static class JsonConfiguration
{
    /// <summary>
    /// Gets the standard JsonSerializerOptions used throughout the application
    /// </summary>
    /// <returns>Configured JsonSerializerOptions with enum string converters</returns>
    public static JsonSerializerOptions GetStandardOptions()
    {
        var options = new JsonSerializerOptions
        {
            // Use PascalCase to match C# property names exactly
            PropertyNamingPolicy = null,
            WriteIndented = true
        };
        
        // Add custom converters for enums to serialize as strings
        options.Converters.Add(new EnumStringJsonConverter<Enums.TaskStatus>());
        options.Converters.Add(new EnumStringJsonConverter<OperationType>());
        
        return options;
    }
    
    /// <summary>
    /// Gets JsonSerializerOptions optimized for API responses (camelCase naming)
    /// </summary>
    /// <returns>Configured JsonSerializerOptions for API responses</returns>
    public static JsonSerializerOptions GetApiOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        // Add custom converters for enums to serialize as strings
        options.Converters.Add(new EnumStringJsonConverter<Enums.TaskStatus>());
        options.Converters.Add(new EnumStringJsonConverter<OperationType>());
        
        return options;
    }
}
