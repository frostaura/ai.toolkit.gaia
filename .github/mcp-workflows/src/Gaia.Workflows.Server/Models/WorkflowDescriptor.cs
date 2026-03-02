namespace Gaia.Workflows.Server.Models;

public sealed class WorkflowDescriptor
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string FilePath { get; init; }
    public List<WorkflowParam> Params { get; init; } = new();
    public string? Output { get; init; }
}

public sealed class WorkflowParam
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}
