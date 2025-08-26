namespace FrostAura.MCP.Gaia.Enums;

/// <summary>
/// Status of a Task item
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task is planned but not started
    /// </summary>
    Todo = 0,

    /// <summary>
    /// Task is in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task is completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Task is blocked or on hold
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Task has been cancelled
    /// </summary>
    Cancelled = 4
}
