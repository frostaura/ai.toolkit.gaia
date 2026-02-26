using System.ComponentModel;
using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Validation;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

public sealed class TasksTool
{
    private readonly JsonTaskStore _store;

    public TasksTool(JsonTaskStore store)
    {
        _store = store;
    }

    [McpServerTool(Name = "tasks_create"), Description(
        "Create a new task in the Gaia task graph for a project. The Workload Orchestrator uses this " +
        "to break a plan into trackable work items. Each task gets a unique id, starts as 'todo', and " +
        "can optionally require gates (e.g. 'ci-green', 'docs-updated') that must be satisfied before " +
        "mark_done succeeds. Example: after Repo Explorer survey, Orchestrator calls tasks_create with " +
        "project='my-api', title='Add Playwright specs for login flow', requiredGates=['ci-green','docs-updated'].")]
    public async Task<TaskItem> Create(string project, string title, string[]? requiredGates = null)
    {
        TaskItem task = null!;
        await _store.MutateAsync(project, tasks =>
        {
            task = new TaskItem
            {
                Id = Guid.NewGuid().ToString("N"),
                Project = project,
                Title = title,
                Status = "todo",
                RequiredGates = requiredGates?.ToList() ?? new()
            };
            tasks.Add(task);
        });
        return task;
    }

    [McpServerTool(Name = "tasks_list"), Description(
        "List all tasks for a project, including their status, blockers, gates, and proof args. " +
        "Use at the start of every orchestration cycle to understand current state before planning " +
        "next actions. Example: Orchestrator calls tasks_list(project='my-api') to see which tasks " +
        "are 'todo', 'doing', or 'done', and whether any have unresolved blockers or NEEDS_INPUT flags.")]
    public async Task<List<TaskItem>> List(string project) => await _store.LoadAsync(project);

    [McpServerTool(Name = "tasks_update"), Description(
        "Update mutable fields on an existing task: title, status ('todo'/'doing'/'done'), " +
        "gates_satisfied, or blockers. Use to transition a task to 'doing' when work begins, " +
        "record gate satisfaction as verification steps pass, or manage blockers. " +
        "Example: Developer starts implementing, Orchestrator calls tasks_update(project='my-api', " +
        "id='abc123', status='doing'). Later, after CI passes: tasks_update(..., gatesSatisfied=['ci-green']).")]
    public async Task<TaskItem> Update(
        string project,
        string id,
        string? title = null,
        string? status = null,
        string[]? gatesSatisfied = null,
        string[]? blockers = null)
    {
        TaskItem result = null!;
        await _store.MutateAsync(project, tasks =>
        {
            var task = tasks.Single(t => t.Id == id);
            if (title is not null) task.Title = title;
            if (status is not null) task.Status = status;
            if (gatesSatisfied is not null) task.GatesSatisfied = gatesSatisfied.ToList();
            if (blockers is not null) task.Blockers = blockers.ToList();
            result = task;
        });
        return result;
    }

    [McpServerTool(Name = "tasks_mark_done"), Description(
        "Mark a task as done. Enforces Gaia completion policy: all blockers must be resolved, " +
        "required gates satisfied, and proof args (changed_files, tests_added, manual_regression " +
        "labels) must be provided. Returns a structured error with code and message if " +
        "validation fails, so the agent can fix issues and retry. " +
        "Example: tasks_mark_done(project='my-api', id='abc123', changedFiles=['src/Login.cs'], " +
        "testsAdded=['tests/LoginTests.cs'], manualRegressionLabels=['curl','playwright-mcp']).")]
    public async Task<object> MarkDone(
        string project,
        string id,
        string[] changedFiles,
        string[] testsAdded,
        string[] manualRegressionLabels)
    {
        object response = null!;
        await _store.MutateAsync(project, tasks =>
        {
            var task = tasks.Single(t => t.Id == id);

            // Validate with candidate proof before mutating the task.
            var candidateProof = new ProofArgs
            {
                ChangedFiles = changedFiles.ToList(),
                TestsAdded = testsAdded.ToList(),
                ManualRegression = manualRegressionLabels.ToList()
            };
            var original = task.Proof;
            task.Proof = candidateProof;

            var err = CompletionValidator.ValidateMarkDone(task);
            if (err is not null)
            {
                task.Proof = original; // Revert — keep mutation atomic.
                response = new { ok = false, error = new { code = err.Code, message = err.Message } };
                return;
            }

            task.Status = "done";
            response = new { ok = true, task_id = id };
        });
        return response;
    }

    [McpServerTool(Name = "tasks_flag_needs_input"), Description(
        "Flag a task as blocked on human input by adding NEEDS_INPUT blockers. The task cannot " +
        "be marked done until these blockers are resolved via tasks_update. Use when an agent " +
        "encounters ambiguity that only a human can resolve. " +
        "Example: Analyst is unsure whether a use-case change is breaking: " +
        "tasks_flag_needs_input(project='my-api', id='abc123', questions=['Is removing the /v1 " +
        "endpoint a breaking change? Should we keep a redirect?']).")]
    public async Task<TaskItem> FlagNeedsInput(string project, string id, string[] questions)
    {
        TaskItem result = null!;
        await _store.MutateAsync(project, tasks =>
        {
            var task = tasks.Single(t => t.Id == id);
            foreach (var q in questions)
            {
                task.Blockers.Add($"NEEDS_INPUT: {q}");
            }
            result = task;
        });
        return result;
    }

    [McpServerTool(Name = "tasks_delete"), Description(
        "Permanently delete a task from a project's task graph. Use when a task was created in " +
        "error or is no longer relevant after a plan revision. Prefer resolving blockers and " +
        "marking done over deleting whenever possible. " +
        "Example: Orchestrator discovers a duplicate task after re-planning: " +
        "tasks_delete(project='my-api', id='dup456').")]
    public async Task<object> Delete(string project, string id)
    {
        var removed = false;
        await _store.MutateAsync(project, tasks =>
        {
            removed = tasks.RemoveAll(t => t.Id == id) > 0;
        });
        return new { ok = removed, project, id };
    }

    [McpServerTool(Name = "tasks_clear"), Description(
        "Clear the entire task graph for a project, removing all tasks. Use only when starting " +
        "a completely fresh planning cycle or resetting a project. This is destructive and " +
        "irreversible. Example: Orchestrator resets after a major scope change: " +
        "tasks_clear(project='my-api').")]
    public async Task<object> ClearAll(string project)
    {
        await _store.SaveAsync(project, new());
        return new { ok = true, project };
    }
}
