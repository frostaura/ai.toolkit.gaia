using Gaia.Mcp.Server.Models;

namespace Gaia.Mcp.Server.Validation;

public static class CompletionValidator
{
    public static ToolError? ValidateComplete(TaskItem task)
    {
        // Check NEEDS_INPUT blockers first for a more specific error message
        if (task.Blockers.Any(b => b.StartsWith("NEEDS_INPUT:", StringComparison.OrdinalIgnoreCase)))
        {
            var needsInput = task.Blockers.Where(b => b.StartsWith("NEEDS_INPUT:", StringComparison.OrdinalIgnoreCase)).ToList();
            return new ToolError(
                ErrorCodes.NeedsInputUnresolved,
                $"Task '{task.Id}' has unresolved human input requests: {string.Join(" | ", needsInput)}. " +
                "Resolve these via tasks_update(blockers=[]) before calling tasks_complete."
            );
        }

        // Then check remaining generic blockers
        if (task.Blockers.Count > 0)
        {
            return new ToolError(
                ErrorCodes.BlockersUnresolved,
                $"Task '{task.Id}' has unresolved blockers: {string.Join(" | ", task.Blockers)}. " +
                "Clear blockers via tasks_update(blockers=[]) before calling tasks_complete."
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
                $"tasks_complete requires all three proof arrays to be non-empty. Missing: {string.Join(", ", missing)}. " +
                "Each array must contain at least one entry (file path or label)."
            );
        }

        // Gate satisfaction check
        var missingGates = task.RequiredGates.Except(task.GatesSatisfied).ToList();
        if (missingGates.Count > 0)
        {
            return new ToolError(
                ErrorCodes.GatesUnsatisfied,
                $"Required gates not satisfied: {string.Join(", ", missingGates)}. " +
                "Call tasks_update(gatesSatisfied=[...]) before tasks_complete."
            );
        }

        return null;
    }
}
