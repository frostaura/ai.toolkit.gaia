namespace Gaia.Mcp.Server.Models;

public sealed class EvolutionItem
{
    public required string Id { get; init; }
    public required string Project { get; init; }
    public required string Suggestion { get; set; }
    public string? Category { get; set; }
    public bool Applied { get; set; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
}
