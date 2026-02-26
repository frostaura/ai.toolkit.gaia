using System.ComponentModel;
using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

/// <summary>
/// Global self-improvement store. Logs process improvement suggestions with a project field.
/// Persisted to a single global JSON file keyed by "__global".
/// </summary>
public sealed class SelfImproveTool
{
    private const string GlobalKey = "__global";
    private readonly ThreadSafeJsonStore<ImprovementItem> _store;

    public SelfImproveTool(ThreadSafeJsonStore<ImprovementItem> store)
    {
        _store = store;
    }

    [McpServerTool(Name = "self_improve_log"), Description(
        "Log a self-improvement suggestion so you and other Gaia agents learn from mistakes and " +
        "get better over time. Call this whenever you identify a recurring problem, a workflow " +
        "inefficiency, a pattern worth codifying, or a lesson learned from a failed attempt. " +
        "The project field provides context; category helps group suggestions " +
        "(e.g. 'workflow', 'testing', 'ci', 'documentation', 'loop-breaker'). " +
        "Example: Quality Gatekeeper notices CI keeps failing on forgotten lint step: " +
        "self_improve_log(project='my-api', suggestion='Add lint gate to required_gates for all " +
        "tasks to prevent CI failures from unfixed lint issues', category='ci').")]
    public async Task<ImprovementItem> Log(
        [Description("Project identifier providing context for the suggestion. Links the improvement to a specific project so it can be filtered and reviewed per-project.")] string project,
        [Description("The improvement suggestion text. Should be a clear, actionable description of what to change or adopt. Describe the problem observed and the recommended fix. Example: 'Add lint gate to required_gates for all tasks to prevent CI failures'.")] string suggestion,
        [Description("Optional category label to group suggestions for easier filtering. Recommended categories: 'workflow', 'testing', 'ci', 'documentation', 'tool-usage', 'loop-breaker'. Use 'loop-breaker' specifically for escape strategies when stuck in recurring blockers. Omit if no category applies.")] string? category = null)
    {
        var item = new ImprovementItem
        {
            Id = Guid.NewGuid().ToString("N"),
            Project = project,
            Suggestion = suggestion,
            Category = category
        };
        await _store.MutateAsync(GlobalKey, items => items.Add(item));
        return item;
    }

    [McpServerTool(Name = "self_improve_list"), Description(
        "Review your self-improvement backlog. Call this at the start of every planning cycle to " +
        "learn from past mistakes and incorporate lessons before repeating them. Optionally filter " +
        "by project or category. Check 'loop-breaker' category when you find yourself stuck on a " +
        "recurring blocker — your past self may have already logged a way out. " +
        "Example: Orchestrator reviews lessons learned before planning: " +
        "self_improve_list() for all, or self_improve_list(category='loop-breaker') for escape tips.")]
    public async Task<List<ImprovementItem>> List(
        [Description("Optional project identifier to filter suggestions. Only suggestions logged for this project are returned. Omit to return suggestions across all projects.")] string? project = null,
        [Description("Optional category label to filter suggestions (e.g. 'workflow', 'ci', 'loop-breaker'). Case-insensitive matching. Omit to return all categories.")] string? category = null)
    {
        var items = await _store.LoadAsync(GlobalKey);
        if (project is not null)
        {
            items = items.Where(i => i.Project == project).ToList();
        }
        if (category is not null)
        {
            items = items.Where(i => string.Equals(i.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return items;
    }

    [McpServerTool(Name = "self_improve_mark_applied"), Description(
        "Mark a self-improvement suggestion as applied after you have acted on it. This keeps " +
        "your improvement backlog clean and records what you have learned and adopted. " +
        "Example: after Orchestrator adds a lint gate to the default required_gates list based " +
        "on a past lesson, it calls self_improve_mark_applied(id='abc123') to close the loop.")]
    public async Task<object> MarkApplied(
        [Description("The unique ID (32-char hex string) of the self-improvement suggestion to mark as applied. Obtain this from the id field in self_improve_list results.")] string id)
    {
        var found = false;
        await _store.MutateAsync(GlobalKey, items =>
        {
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item is not null)
            {
                item.Applied = true;
                found = true;
            }
        });
        return new { ok = found, id };
    }

    [McpServerTool(Name = "self_improve_clear"), Description(
        "Clear self-improvement suggestions from the global store. Pass a project to clear only " +
        "that project's suggestions, or omit to wipe all. Use sparingly — only when old lessons " +
        "are no longer relevant after a major process overhaul. " +
        "Example: after a full process review, clear stale lessons for a retired project: " +
        "self_improve_clear(project='old-service').")]
    public async Task<object> Clear(
        [Description("Optional project identifier. If provided, only suggestions for this project are removed. If omitted, ALL suggestions across all projects are wiped. Use with caution when omitting — this clears the entire global improvement backlog.")] string? project = null)
    {
        if (project is null)
        {
            await _store.SaveAsync(GlobalKey, new());
        }
        else
        {
            await _store.MutateAsync(GlobalKey, items =>
            {
                items.RemoveAll(i => i.Project == project);
            });
        }
        return new { ok = true, project = project ?? "all" };
    }
}
