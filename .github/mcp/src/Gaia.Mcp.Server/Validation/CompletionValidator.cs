using Gaia.Mcp.Server.Models;

namespace Gaia.Mcp.Server.Validation;

public static class CompletionValidator
{
    public static ToolError? ValidateMarkDone(TaskItem task, string repoRoot)
    {
        if (task.Blockers.Count > 0)
        {
            return new ToolError(
                ErrorCodes.BlockersUnresolved,
                $"Task '{task.Id}' has unresolved blockers: {string.Join(" | ", task.Blockers)}"
            );
        }

        // Proof args required (paths/labels only)
        if (task.Proof.ChangedFiles.Count == 0 || task.Proof.TestsAdded.Count == 0)
        {
            return new ToolError(
                ErrorCodes.MissingProofArgs,
                "mark_done requires proof args: changed_files[], tests_added[], manual_regression[] (labels)."
            );
        }

        // Validate proof paths exist
        var missing = new List<string>();
        foreach (var p in task.Proof.ChangedFiles.Concat(task.Proof.TestsAdded))
        {
            var full = System.IO.Path.Combine(repoRoot, p);
            if (!File.Exists(full) && !Directory.Exists(full)) missing.Add(p);
        }
        if (missing.Count > 0)
        {
            return new ToolError(
                ErrorCodes.MissingProofPaths,
                $"Proof references missing paths: {string.Join(", ", missing)}"
            );
        }

        // Gate satisfaction check
        var missingGates = task.RequiredGates.Except(task.GatesSatisfied).ToList();
        if (missingGates.Count > 0)
        {
            return new ToolError(
                ErrorCodes.GatesUnsatisfied,
                $"Required gates not satisfied: {string.Join(", ", missingGates)}. Update gates_satisfied[] before mark_done."
            );
        }

        // Needs-input unresolved is modeled as blockers; keep explicit code option
        if (task.Blockers.Any(b => b.StartsWith("NEEDS_INPUT:", StringComparison.OrdinalIgnoreCase)))
        {
            return new ToolError(
                ErrorCodes.NeedsInputUnresolved,
                "Needs human input is still unresolved. Resolve blockers or remove NEEDS_INPUT entries before mark_done."
            );
        }

        return null;
    }
}
