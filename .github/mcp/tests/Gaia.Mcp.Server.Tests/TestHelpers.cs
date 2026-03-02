using System.Text.Json;

namespace Gaia.Mcp.Server.Tests;

internal static class TestHelpers
{
    /// <summary>
    /// Converts an anonymous object to JsonElement so we can inspect properties
    /// returned by methods that return `object` (anonymous types are internal).
    /// </summary>
    public static JsonElement ToJson(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonDocument.Parse(json).RootElement;
    }
}
