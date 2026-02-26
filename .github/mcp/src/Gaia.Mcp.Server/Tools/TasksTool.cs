using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Validation;

namespace Gaia.Mcp.Server.Tools;

// Tool methods are designed to be wired into an MCP host.
public sealed class TasksTool
{
    private readonly JsonTaskStore _store;
    private readonly string _repoRoot;

    public TasksTool(JsonTaskStore store, string repoRoot)
    {
        _store = store;
        _repoRoot = repoRoot;
    }

    public TaskItem Create(string project, string title, IEnumerable<string>? requiredGates = null)
    {
        var tasks = _store.Load(project);
        var task = new TaskItem
        {
            Id = Guid.NewGuid().ToString("N"),
            Project = project,
            Title = title,
            Status = "todo",
            RequiredGates = requiredGates?.ToList() ?? new()
        };
        tasks.Add(task);
        _store.Save(project, tasks);
        return task;
    }

    public List<TaskItem> List(string project) => _store.Load(project);

    public TaskItem Update(string project, string id, Action<TaskItem> mutate)
    {
        var tasks = _store.Load(project);
        var task = tasks.Single(t => t.Id == id);
        mutate(task);
        _store.Save(project, tasks);
        return task;
    }

    public object MarkDone(
        string project,
        string id,
        IEnumerable<string> changedFiles,
        IEnumerable<string> testsAdded,
        IEnumerable<string> manualRegressionLabels)
    {
        var tasks = _store.Load(project);
        var task = tasks.Single(t => t.Id == id);

        task.Proof.ChangedFiles = changedFiles.ToList();
        task.Proof.TestsAdded = testsAdded.ToList();
        task.Proof.ManualRegression = manualRegressionLabels.ToList();

        var err = CompletionValidator.ValidateMarkDone(task, _repoRoot);
        if (err is not null)
        {
            // Return a structured error so agents can reason.
            return new { ok = false, error = new { code = err.Code, message = err.Message } };
        }

        task.Status = "done";
        _store.Save(project, tasks);
        return new { ok = true, task_id = id };
    }

    public TaskItem FlagNeedsInput(string project, string id, IEnumerable<string> questions)
    {
        return Update(project, id, t =>
        {
            foreach (var q in questions)
            {
                t.Blockers.Add($"NEEDS_INPUT: {q}");
            }
        });
    }
}
