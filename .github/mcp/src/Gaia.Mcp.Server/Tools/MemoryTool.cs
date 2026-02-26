using System.ComponentModel;
using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

/// <summary>
/// Project-scoped stable memory storage. Persists facts (how to run, env vars, conventions)
/// as key-value pairs in a per-project JSON file.
/// </summary>
public sealed class MemoryTool
{
    private readonly ThreadSafeJsonStore<MemoryItem> _store;

    public MemoryTool(ThreadSafeJsonStore<MemoryItem> store)
    {
        _store = store;
    }

    [McpServerTool(Name = "memory_remember"), Description(
        "Persist a stable fact about a project so all Gaia agents can recall it across sessions. " +
        "Store only durable knowledge: build commands, environment variables, repo conventions, " +
        "tech stack details, or verified patterns. If the key already exists, its value is updated " +
        "(upsert). Keys should be descriptive and namespaced (e.g. 'build/command', 'env/DATABASE_URL'). " +
        "Example: after Repo Explorer discovers the build command, it calls " +
        "memory_remember(project='my-api', key='build/command', value='dotnet build src/Api.csproj').")]
    public async Task<MemoryItem> Remember(string project, string key, string value)
    {
        MemoryItem result = null!;
        await _store.MutateAsync(project, items =>
        {
            var existing = items.FirstOrDefault(m => m.Key == key);
            if (existing is not null)
            {
                existing.Value = value;
                existing.UpdatedUtc = DateTime.UtcNow;
                result = existing;
            }
            else
            {
                var item = new MemoryItem
                {
                    Key = key,
                    Value = value,
                    Project = project
                };
                items.Add(item);
                result = item;
            }
        });
        return result;
    }

    [McpServerTool(Name = "memory_recall"), Description(
        "Recall all stored facts for a project, or filter by key prefix. Every Gaia agent should " +
        "call this at the start of a session to load project context (build commands, conventions, " +
        "env vars) before doing work. Use key to scope recall to a namespace (prefix match). " +
        "Example: Developer agent starts work and calls memory_recall(project='my-api') to get all " +
        "facts, or memory_recall(project='my-api', key='env/') to get only environment variables.")]
    public async Task<List<MemoryItem>> Recall(string project, string? key = null)
    {
        var items = await _store.LoadAsync(project);
        if (key is not null)
        {
            items = items.Where(m => m.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return items;
    }

    [McpServerTool(Name = "memory_forget"), Description(
        "Remove a single fact by exact key from a project's memory. Use when a previously stored " +
        "fact is no longer accurate (e.g. a build command changed, an env var was removed). " +
        "Keeps memory clean and prevents agents from acting on stale information. " +
        "Example: after migrating from npm to pnpm, call " +
        "memory_forget(project='my-app', key='build/command') then re-store the updated command.")]
    public async Task<object> Forget(string project, string key)
    {
        var removed = false;
        await _store.MutateAsync(project, items =>
        {
            var count = items.RemoveAll(m => m.Key == key);
            removed = count > 0;
        });
        return new { ok = removed, project, key };
    }

    [McpServerTool(Name = "memory_clear"), Description(
        "Clear all stored facts for a project. Use when a project's context is significantly " +
        "outdated and a full re-survey is needed, or when onboarding a project from scratch. " +
        "This is destructive — all memorized conventions, build commands, and env vars are lost. " +
        "Example: after a major repo restructure, Orchestrator calls " +
        "memory_clear(project='my-api') then triggers a fresh Repo Explorer survey.")]
    public async Task<object> Clear(string project)
    {
        await _store.SaveAsync(project, new());
        return new { ok = true, project };
    }
}
