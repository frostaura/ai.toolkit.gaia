using System.ComponentModel;
using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

/// <summary>
/// Global evolution store. Logs lessons and process upgrades agents can leverage to
/// continuously evolve themselves. Persisted to a single global JSON file keyed by "__global".
/// </summary>
public sealed class EvolveTool
{
    private const string GlobalKey = "__global";
    private readonly ThreadSafeJsonStore<EvolutionItem> _store;

    public EvolveTool(ThreadSafeJsonStore<EvolutionItem> store)
    {
        _store = store;
    }

    [McpServerTool(Name = "evolve_log"), Description(
        "Provides an append-only entry in Gaia's global evolution backlog so all agents can compound lessons learned and continuously upgrade themselves. " +
        "How to use: pass project (context anchor), a clear actionable suggestion describing the problem and recommended fix, and an optional category ('workflow', 'testing', 'ci', 'documentation', 'tool-usage', 'loop-breaker'); returns the stored EvolutionItem with a generated id. " +
        "Use whenever you spot a recurring problem, a workflow inefficiency, a pattern worth codifying, or a lesson from a failed attempt \u2014 capture it the moment you notice it, not later. " +
        "Example: evolve_log(project='my-api', suggestion='Add lint gate to required_gates for all tasks to prevent CI failures from unfixed lint issues', category='ci').")]
    public async Task<EvolutionItem> Log(
        [Description("Project identifier providing context for the suggestion. Links the evolution to a specific project so it can be filtered and reviewed per-project.")] string project,
        [Description("The evolution suggestion text. Should be a clear, actionable description of what to change or adopt. Describe the problem observed and the recommended fix. Example: 'Add lint gate to required_gates for all tasks to prevent CI failures'.")] string suggestion,
        [Description("Optional category label to group suggestions for easier filtering. Recommended categories: 'workflow', 'testing', 'ci', 'documentation', 'tool-usage', 'loop-breaker'. Use 'loop-breaker' specifically for escape strategies when stuck in recurring blockers. Omit if no category applies.")] string? category = null)
    {
        var item = new EvolutionItem
        {
            Id = Guid.NewGuid().ToString("N"),
            Project = project,
            Suggestion = suggestion,
            Category = category
        };
        await _store.MutateAsync(GlobalKey, items => items.Add(item));
        return item;
    }

    [McpServerTool(Name = "evolve_list"), Description(
        "Provides the global evolution backlog \u2014 every logged lesson, optionally filtered by project and/or category \u2014 so agents can incorporate past learnings before repeating mistakes. " +
        "How to use: call with no args to see all suggestions; pass project to scope to one project; pass category (case-insensitive) to focus on a class of lesson, especially 'loop-breaker' when stuck. " +
        "Use at the start of every planning cycle, before tackling a recurring blocker, and any time you suspect 'someone may have already solved this'. " +
        "Example: evolve_list() for everything; evolve_list(category='loop-breaker') when stuck.")]
    public async Task<List<EvolutionItem>> List(
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

    [McpServerTool(Name = "evolve_apply"), Description(
        "Provides closure on an evolution suggestion by flipping its 'applied' flag, keeping the backlog focused on lessons not yet absorbed. " +
        "How to use: pass the suggestion's id (32-char hex from evolve_list); returns {ok, id} where ok indicates whether the id was found. The entry stays in history \u2014 only its applied flag changes. " +
        "Use immediately after acting on a suggestion (codifying it in a skill, agent, gate, or process) so future evolve_list reviews can ignore it. " +
        "Example: evolve_apply(id='abc123').")]
    public async Task<object> Apply(
        [Description("The unique ID (32-char hex string) of the evolution suggestion to mark as applied. Obtain this from the id field in evolve_list results.")] string id)
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

    [McpServerTool(Name = "evolve_clear"), Description(
        "Provides bulk deletion of evolution suggestions \u2014 either every suggestion for one project, or the entire global backlog when project is omitted. " +
        "How to use: pass project to scope to one project; omit project to wipe everything. The operation is destructive and irreversible \u2014 confirm with the human before calling, especially without a project filter. " +
        "Use only after a major process overhaul renders old lessons obsolete, or when retiring a project; never as a tidy-up step (use evolve_apply for that). " +
        "Example: evolve_clear(project='old-service').")]
    public async Task<object> Clear(
        [Description("Optional project identifier. If provided, only suggestions for this project are removed. If omitted, ALL suggestions across all projects are wiped. Use with caution when omitting — this clears the entire global evolution backlog.")] string? project = null)
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
