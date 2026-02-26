namespace Gaia.Mcp.Server.Models;

public sealed class ProofArgs
{
    public List<string> ChangedFiles { get; set; } = new();
    public List<string> TestsAdded { get; set; } = new();
    public List<string> ManualRegression { get; set; } = new();
}
