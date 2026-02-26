namespace Gaia.Mcp.Server.Models;

public sealed class MemoryItem
{
    public required string Key { get; init; }
    public required string Value { get; set; }
    public required string Project { get; init; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
