using System.Text.Json;
using Gaia.Mcp.Server.Models;

namespace Gaia.Mcp.Server.Storage;

public sealed class JsonTaskStore
{
    private readonly string _root;

    public JsonTaskStore(string rootDir)
    {
        _root = rootDir;
        Directory.CreateDirectory(_root);
    }

    private string PathFor(string project) => System.IO.Path.Combine(_root, $"{project}.tasks.json");

    public List<TaskItem> Load(string project)
    {
        var path = PathFor(project);
        if (!File.Exists(path)) return new();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new();
    }

    public void Save(string project, List<TaskItem> tasks)
    {
        var path = PathFor(project);
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
