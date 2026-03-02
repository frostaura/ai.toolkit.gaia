namespace Gaia.Mcp.Server.Models;

public static class ErrorCodes
{
    public const string MissingProofArgs = "GAIA_TASKS_ERR_MISSING_PROOF_ARGS";
    public const string BlockersUnresolved = "GAIA_TASKS_ERR_BLOCKERS_UNRESOLVED";
    public const string GatesUnsatisfied = "GAIA_TASKS_ERR_GATES_UNSATISFIED";
    public const string NeedsInputUnresolved = "GAIA_TASKS_ERR_NEEDS_INPUT_UNRESOLVED";
    public const string TaskNotFound = "GAIA_TASKS_ERR_TASK_NOT_FOUND";
    public const string InvalidStatus = "GAIA_TASKS_ERR_INVALID_STATUS";
}
