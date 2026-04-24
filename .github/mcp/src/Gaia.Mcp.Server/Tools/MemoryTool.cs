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
        "Provides durable, project-scoped fact storage so every Gaia agent can recall the same knowledge across sessions; if the key already exists, its value is overwritten (upsert). " +
        "How to use: pass project, a descriptive namespaced key (e.g. 'build/command', 'env/DATABASE_URL'), and the value to store; returns the stored MemoryItem. " +
        "Use only for stable facts \u2014 build commands, env vars, repo conventions, tech stack details, verified patterns. Do not use for ephemeral working state (use session memory or tasks for that). " +
        "Example: memory_remember(project='my-api', key='build/command', value='dotnet build src/Api.csproj').")]
    public async Task<MemoryItem> Remember(
        [Description("Project identifier that scopes this memory entry. Must match the project name used across all Gaia tools (tasks, memory, evolve) for consistency. Example: 'my-api'.")] string project,
        [Description("Descriptive, namespaced key for the fact being stored. Use '/' to create logical namespaces (e.g. 'build/command', 'env/DATABASE_URL', 'convention/branch-naming', 'stack/language'). If the key already exists, its value is updated (upsert semantics). Keys are case-sensitive.")] string key,
        [Description("The fact value to store. Should be a concise, durable piece of knowledge — a command, a URL, a convention description, or a config value. Avoid storing ephemeral or session-specific information.")] string value)
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
        "Provides stored facts for a project, ordered by most-recently-updated first, optionally filtered by key prefix and capped to a limit. " +
        "How to use: pass project; optionally pass a key prefix (case-insensitive, e.g. 'env/') and limit (default 25); returns the matching MemoryItem list. " +
        "Use at the start of every agent session to load project context (build commands, conventions, env vars) before doing work, and any time you need to look up a previously verified fact. " +
        "Example: memory_recall(project='my-api') for everything; memory_recall(project='my-api', key='env/') for env vars only.")]
    public async Task<List<MemoryItem>> Recall(
        [Description("Project identifier to recall facts for. Returns stored facts for this project only.")] string project,
        [Description("Optional key or key prefix to filter results. If provided, returns only facts whose key starts with this value (prefix match, case-insensitive). Omit to return all facts for the project. Example: 'env/' returns all env-namespaced facts.")] string? key = null,
        [Description("Maximum number of facts to return (default: 25). Results are ordered by most recently updated first, so this returns the top N most relevant/recent facts.")] int limit = 25)
    {
        var items = await _store.LoadAsync(project);
        if (key is not null)
        {
            items = items.Where(m => m.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return items
            .OrderByDescending(m => m.UpdatedUtc)
            .Take(limit)
            .ToList();
    }

    [McpServerTool(Name = "memory_forget"), Description(
        "Provides removal of a single stored fact identified by exact key, keeping memory clean and preventing agents from acting on stale information. " +
        "How to use: pass project + the exact case-sensitive key (use memory_recall first if unsure); returns {ok, project, key} where ok indicates whether a row was removed. " +
        "Use when a previously stored fact is no longer accurate (build command changed, env var removed, convention superseded), typically followed by a memory_remember with the new value. " +
        "Example: memory_forget(project='my-app', key='build/command').")]
    public async Task<object> Forget(
        [Description("Project identifier the fact belongs to.")] string project,
        [Description("Exact key of the fact to remove. Must match the stored key exactly (case-sensitive). Use memory_recall to discover existing keys if unsure.")] string key)
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
        "Provides a full wipe of a project's memory store \u2014 every conventions, build command, env var, and other fact for that project is permanently deleted. " +
        "How to use: pass project; the operation is destructive and irreversible \u2014 confirm with the human before calling. " +
        "Use only after a major repo restructure or when onboarding a project from scratch and a fresh Repo Explorer survey will repopulate the store; never as a tidy-up step. " +
        "Example: memory_clear(project='my-api').")]
    public async Task<object> Clear(
        [Description("Project identifier whose entire memory store will be wiped. This is destructive — all stored conventions, build commands, env vars, and other facts for this project are permanently deleted.")] string project)
    {
        await _store.SaveAsync(project, new());
        return new { ok = true, project };
    }
}
