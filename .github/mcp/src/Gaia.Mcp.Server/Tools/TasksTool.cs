using System.ComponentModel;
using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Validation;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

public sealed class TasksTool
{
    private static readonly HashSet<string> s_validStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "todo", "doing", "done"
    };

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
    public async Task<TaskItem> Create(
        [Description("Project identifier that scopes this task. Must match the project name used across all Gaia tools (tasks, memory, self-improve) for consistency. Example: 'my-api'.")] string project,
        [Description("Short, action-oriented title summarizing what this task accomplishes. Should be specific enough to be actionable without reading the description. Example: 'Add Playwright specs for login flow'.")] string title,
        [Description("Optional longer description providing context, acceptance criteria, or implementation notes for the task. Omit if the title is self-explanatory.")] string? description = null,
        [Description("Optional list of gate labels that must ALL be satisfied before tasks_mark_done will accept this task as complete. Each gate is a string label (e.g. 'ci-green', 'docs-updated', 'lint-clean'). Gates are satisfied by calling tasks_update with gatesSatisfied. If omitted, no gates are enforced.")] string[]? requiredGates = null)
    {
        TaskItem task = null!;
        await _store.MutateAsync(project, tasks =>
        {
            task = new TaskItem
            {
                Id = Guid.NewGuid().ToString("N"),
                Project = project,
                Title = title,
                Description = description,
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
    public async Task<List<TaskItem>> List(
        [Description("Project identifier to list tasks for. Returns all tasks regardless of status (todo, doing, done).")] string project) => await _store.LoadAsync(project);

    [McpServerTool(Name = "tasks_update"), Description(
        "Update mutable fields on an existing task: title, status ('todo'/'doing'/'done'), " +
        "gates_satisfied, or blockers. Use to transition a task to 'doing' when work begins, " +
        "record gate satisfaction as verification steps pass, or manage blockers. " +
        "Example: Developer starts implementing, Orchestrator calls tasks_update(project='my-api', " +
        "id='abc123', status='doing'). Later, after CI passes: tasks_update(..., gatesSatisfied=['ci-green']).")]
    public async Task<object> Update(
        [Description("Project identifier the task belongs to.")] string project,
        [Description("The unique task ID (32-char hex string) returned by tasks_create. Must match exactly.")] string id,
        [Description("New title for the task. Pass null/omit to keep the current title unchanged.")] string? title = null,
        [Description("New description for the task. Pass null/omit to keep the current description unchanged.")] string? description = null,
        [Description("New status for the task. Allowed values: 'todo', 'doing', 'done'. Note: prefer tasks_mark_done over setting status to 'done' directly, as mark_done enforces proof and gate validation. Pass null/omit to keep current status.")] string? status = null,
        [Description("List of gate labels that must be satisfied before mark_done succeeds (e.g. ['ci-green', 'docs-updated']). Replaces the entire requiredGates list. Pass null/omit to keep current required gates unchanged.")] string[]? requiredGates = null,
        [Description("List of gate labels now satisfied (e.g. ['ci-green', 'docs-updated']). Replaces the entire gatesSatisfied list. Must be a subset of the task's requiredGates. Pass null/omit to keep current gates unchanged.")] string[]? gatesSatisfied = null,
        [Description("List of blocker strings. Replaces the entire blockers list. Use to add or clear blockers. To clear all blockers, pass an empty array []. Unresolved blockers prevent tasks_mark_done from succeeding. Pass null/omit to keep current blockers unchanged.")] string[]? blockers = null)
    {
        if (status is not null && !s_validStatuses.Contains(status))
        {
            return new
            {
                ok = false,
                error = new
                {
                    code = ErrorCodes.InvalidStatus,
                    message = $"Invalid status '{status}'. Allowed values: 'todo', 'doing', 'done'."
                }
            };
        }

        object result = null!;
        await _store.MutateAsync(project, tasks =>
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task is null)
            {
                result = new
                {
                    ok = false,
                    error = new
                    {
                        code = ErrorCodes.TaskNotFound,
                        message = $"Task '{id}' not found in project '{project}'."
                    }
                };
                return;
            }

            if (title is not null) task.Title = title;
            if (description is not null) task.Description = description;
            if (status is not null) task.Status = status;
            if (requiredGates is not null) task.RequiredGates = requiredGates.ToList();
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
        [Description("Project identifier the task belongs to.")] string project,
        [Description("The unique task ID (32-char hex string) returned by tasks_create.")] string id,
        [Description("Non-empty array of file paths that were changed to complete this task. Must contain at least one path. These are recorded as proof of work. Example: ['src/Controllers/HealthController.cs', 'src/Program.cs'].")] string[] changedFiles,
        [Description("Non-empty array of test file paths added or modified for this task. Must contain at least one path — every task must have test coverage. Example: ['tests/HealthControllerTests.cs'].")] string[] testsAdded,
        [Description("Non-empty array of manual regression labels describing how the change was manually verified. Must contain at least one label. Common labels: 'curl', 'playwright-mcp', 'browser-manual'. Example: ['curl', 'playwright-mcp'].")] string[] manualRegressionLabels)
    {
        object response = null!;
        await _store.MutateAsync(project, tasks =>
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task is null)
            {
                response = new
                {
                    ok = false,
                    error = new
                    {
                        code = ErrorCodes.TaskNotFound,
                        message = $"Task '{id}' not found in project '{project}'."
                    }
                };
                return;
            }

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
    public async Task<object> FlagNeedsInput(
        [Description("Project identifier the task belongs to.")] string project,
        [Description("The unique task ID (32-char hex string) of the task to flag.")] string id,
        [Description("Array of one or more question strings that need human answers before work can proceed. Each question becomes a 'NEEDS_INPUT: ...' blocker on the task. These blockers must be cleared (via tasks_update with blockers=[]) before tasks_mark_done will succeed. Example: ['Is removing /v1 a breaking change?', 'Should we add a deprecation notice?'].")] string[] questions)
    {
        object result = null!;
        await _store.MutateAsync(project, tasks =>
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task is null)
            {
                result = new
                {
                    ok = false,
                    error = new
                    {
                        code = ErrorCodes.TaskNotFound,
                        message = $"Task '{id}' not found in project '{project}'."
                    }
                };
                return;
            }

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
    public async Task<object> Delete(
        [Description("Project identifier the task belongs to.")] string project,
        [Description("The unique task ID (32-char hex string) of the task to permanently remove.")] string id)
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
    public async Task<object> ClearAll(
        [Description("Project identifier whose entire task graph will be wiped. This is destructive and irreversible — all tasks, proofs, and blockers for this project are permanently deleted.")] string project)
    {
        await _store.SaveAsync(project, new());
        return new { ok = true, project };
    }
}
