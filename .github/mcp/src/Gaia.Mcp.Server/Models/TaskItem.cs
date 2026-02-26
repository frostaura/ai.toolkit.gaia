namespace Gaia.Mcp.Server.Models;

public sealed class TaskItem
{
    public required string Id { get; init; }
    public required string Project { get; init; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "todo"; // todo|doing|done

    public List<string> RequiredGates { get; set; } = new();
    public List<string> GatesSatisfied { get; set; } = new();

    public List<string> Blockers { get; set; } = new();

    public ProofArgs Proof { get; set; } = new();
}
