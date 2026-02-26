using Gaia.Mcp.Server.Models;

namespace Gaia.Mcp.Server.Validation;

public static class CompletionValidator
{
    public static ToolError? ValidateMarkDone(TaskItem task)
    {
        if (task.Blockers.Count > 0)
        {
            return new ToolError(
                ErrorCodes.BlockersUnresolved,
                $"Task '{task.Id}' has unresolved blockers: {string.Join(" | ", task.Blockers)}"
            );
        }

        // Proof args required (paths/labels only) — all three must be non-empty
        if (task.Proof.ChangedFiles.Count == 0 || task.Proof.TestsAdded.Count == 0 || task.Proof.ManualRegression.Count == 0)
        {
            var missing = new List<string>();
            if (task.Proof.ChangedFiles.Count == 0) missing.Add("changedFiles");
            if (task.Proof.TestsAdded.Count == 0) missing.Add("testsAdded");
            if (task.Proof.ManualRegression.Count == 0) missing.Add("manualRegressionLabels");
            return new ToolError(
                ErrorCodes.MissingProofArgs,
                $"mark_done requires all three proof arrays to be non-empty. Missing: {string.Join(", ", missing)}. " +
                "Each array must contain at least one entry (file path or label)."
            );
        }

        // Proof paths are accepted as labels; no filesystem check.
        // The MCP server may run remotely and cannot access the client's repo.

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
