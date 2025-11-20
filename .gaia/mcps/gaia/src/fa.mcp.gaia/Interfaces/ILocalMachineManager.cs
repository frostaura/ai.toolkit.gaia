namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for local machine operations and information retrieval
/// </summary>
public interface ILocalMachineManager
{
    /// <summary>
    /// Gets the current host machine name
    /// </summary>
    /// <returns>JSON string containing the host machine name and additional system information</returns>
    Task<string> GetHostMachineNameAsync();
}
