using System.Text.Json;
using Gaia.Mcp.Server.Models;

namespace Gaia.Mcp.Server.Storage;

/// <summary>
/// Thread-safe task store backed by per-project JSON files.
/// Delegates to ThreadSafeJsonStore for locking and atomic writes.
/// </summary>
public sealed class JsonTaskStore
{
    private readonly ThreadSafeJsonStore<TaskItem> _inner;

    public JsonTaskStore(string rootDir)
    {
        _inner = new ThreadSafeJsonStore<TaskItem>(rootDir, ".tasks.json");
    }

    public Task<List<TaskItem>> LoadAsync(string project) => _inner.LoadAsync(project);

    public Task SaveAsync(string project, List<TaskItem> tasks) => _inner.SaveAsync(project, tasks);

    public Task<List<TaskItem>> MutateAsync(string project, Action<List<TaskItem>> mutate) =>
        _inner.MutateAsync(project, mutate);
}
