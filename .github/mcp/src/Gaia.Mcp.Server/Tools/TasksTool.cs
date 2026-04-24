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
        "Provides a new tracked task in the Gaia task graph for a project, with a unique id, an initial 'todo' status, and optional gates that must be satisfied before completion. " +
        "How to use: pass project, a short action-oriented title, an optional description, and an optional requiredGates list (e.g. ['ci-green','docs-updated']); the returned TaskItem includes the generated id used by every other tasks_* call. " +
        "Use when the Workload Orchestrator breaks a plan into trackable work items, when new work is discovered mid-flight, or when a NEEDS_INPUT answer reveals additional scope. " +
        "Example: tasks_create(project='my-api', title='Add Playwright specs for login flow', requiredGates=['ci-green','docs-updated']).")]
    public async Task<TaskItem> Create(
        [Description("Project identifier that scopes this task. Must match the project name used across all Gaia tools (tasks, memory, evolve) for consistency. Example: 'my-api'.")] string project,
        [Description("Short, action-oriented title summarizing what this task accomplishes. Should be specific enough to be actionable without reading the description. Example: 'Add Playwright specs for login flow'.")] string title,
        [Description("Optional longer description providing context, acceptance criteria, or implementation notes for the task. Omit if the title is self-explanatory.")] string? description = null,
        [Description("Optional list of gate labels that must ALL be satisfied before tasks_complete will accept this task as complete. Each gate is a string label (e.g. 'ci-green', 'docs-updated', 'lint-clean'). Gates are satisfied by calling tasks_update with gatesSatisfied. If omitted, no gates are enforced.")] string[]? requiredGates = null)
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
        "Provides every task for a project with its current status, blockers, required/satisfied gates, and proof args. " +
        "How to use: pass the project identifier; returns the full task list regardless of status so you can pick what to work on next. " +
        "Use at the start of every orchestration cycle, before planning new work, and whenever you need to confirm whether a task is unblocked or still has NEEDS_INPUT. " +
        "Example: tasks_list(project='my-api').")]
    public async Task<List<TaskItem>> List(
        [Description("Project identifier to list tasks for. Returns all tasks regardless of status (todo, doing, done).")] string project) => await _store.LoadAsync(project);

    [McpServerTool(Name = "tasks_update"), Description(
        "Provides in-place edits to a task's mutable fields (title, description, status, requiredGates, gatesSatisfied, blockers) without altering its id or proof. " +
        "How to use: pass project + id plus only the fields you want to change; omit a field to leave it unchanged, and pass an empty array to clear list fields like blockers. Status must be 'todo', 'doing', or 'done' — prefer tasks_complete over setting 'done' here so proof and gates are validated. " +
        "Use when transitioning a task to 'doing' as work begins, recording gate satisfaction as verifications pass, adding/clearing blockers, or fixing a title after replanning. " +
        "Example: tasks_update(project='my-api', id='abc123', status='doing'); later tasks_update(..., gatesSatisfied=['ci-green']).")]
    public async Task<object> Update(
        [Description("Project identifier the task belongs to.")] string project,
        [Description("The unique task ID (32-char hex string) returned by tasks_create. Must match exactly.")] string id,
        [Description("New title for the task. Pass null/omit to keep the current title unchanged.")] string? title = null,
        [Description("New description for the task. Pass null/omit to keep the current description unchanged.")] string? description = null,
        [Description("New status for the task. Allowed values: 'todo', 'doing', 'done'. Note: prefer tasks_complete over setting status to 'done' directly, as tasks_complete enforces proof and gate validation. Pass null/omit to keep current status.")] string? status = null,
        [Description("List of gate labels that must be satisfied before mark_done succeeds (e.g. ['ci-green', 'docs-updated']). Replaces the entire requiredGates list. Pass null/omit to keep current required gates unchanged.")] string[]? requiredGates = null,
        [Description("List of gate labels now satisfied (e.g. ['ci-green', 'docs-updated']). Replaces the entire gatesSatisfied list. Must be a subset of the task's requiredGates. Pass null/omit to keep current gates unchanged.")] string[]? gatesSatisfied = null,
        [Description("List of blocker strings. Replaces the entire blockers list. Use to add or clear blockers. To clear all blockers, pass an empty array []. Unresolved blockers prevent tasks_complete from succeeding. Pass null/omit to keep current blockers unchanged.")] string[]? blockers = null)
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

    [McpServerTool(Name = "tasks_complete"), Description(
        "Provides authoritative completion of a task by enforcing Gaia's done policy: blockers must be empty, every requiredGate must be satisfied, and proof args must be non-empty. " +
        "How to use: pass project + id and three non-empty arrays \u2014 changedFiles, testsAdded, manualRegressionLabels; on failure returns a structured {ok:false,error:{code,message}} so you can fix the gap and retry. " +
        "Use only after all gates are recorded as satisfied and the work has real test coverage and manual verification \u2014 never use it as a status shortcut. " +
        "Example: tasks_complete(project='my-api', id='abc123', changedFiles=['src/Login.cs'], testsAdded=['tests/LoginTests.cs'], manualRegressionLabels=['curl','playwright-mcp']).")]
    public async Task<object> Complete(
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

            var err = CompletionValidator.ValidateComplete(task);
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

    [McpServerTool(Name = "tasks_request_input"), Description(
        "Provides a structured way to park a task on human input by adding 'NEEDS_INPUT: \u2026' blockers from a list of questions; the task cannot be completed until those blockers are cleared. " +
        "How to use: pass project + id and an array of one or more clear, answerable questions; clear the blockers later via tasks_update(blockers=[]) once the human responds. " +
        "Use when an agent hits ambiguity only a human can resolve (scope, breaking-change calls, missing credentials) instead of guessing or stalling silently. " +
        "Example: tasks_request_input(project='my-api', id='abc123', questions=['Is removing /v1 a breaking change? Should we keep a redirect?']).")]
    public async Task<object> RequestInput(
        [Description("Project identifier the task belongs to.")] string project,
        [Description("The unique task ID (32-char hex string) of the task to flag.")] string id,
        [Description("Array of one or more question strings that need human answers before work can proceed. Each question becomes a 'NEEDS_INPUT: ...' blocker on the task. These blockers must be cleared (via tasks_update with blockers=[]) before tasks_complete will succeed. Example: ['Is removing /v1 a breaking change?', 'Should we add a deprecation notice?'].")] string[] questions)
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
        "Provides permanent removal of a single task from a project's task graph. " +
        "How to use: pass project + id; returns {ok:true|false,...}. There is no soft-delete \u2014 the task and its proof are gone. " +
        "Use only when a task was created in error or became obsolete after replanning; otherwise prefer resolving blockers and marking it done. " +
        "Example: tasks_delete(project='my-api', id='dup456').")]
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
        "Provides a full wipe of a project's task graph, deleting every task, proof, and blocker for that project. " +
        "How to use: pass project; the operation is destructive and irreversible \u2014 confirm with the human before calling. " +
        "Use only at the start of a fully fresh planning cycle or when resetting a project after a major scope change; never as a tidy-up step. " +
        "Example: tasks_clear(project='my-api').")]
    public async Task<object> Clear(
        [Description("Project identifier whose entire task graph will be wiped. This is destructive and irreversible — all tasks, proofs, and blockers for this project are permanently deleted.")] string project)
    {
        await _store.SaveAsync(project, new());
        return new { ok = true, project };
    }
}
