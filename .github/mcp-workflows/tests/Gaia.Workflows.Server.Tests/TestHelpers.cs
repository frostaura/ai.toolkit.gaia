using System.Text.Json;

namespace Gaia.Workflows.Server.Tests;

internal static class TestHelpers
{
    public static JsonElement ToJson(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonDocument.Parse(json).RootElement;
    }
}
