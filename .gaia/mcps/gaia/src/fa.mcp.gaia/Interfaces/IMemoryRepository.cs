using FrostAura.MCP.Gaia.Models;

namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for memory repository operations
/// </summary>
public interface IMemoryRepository
{
    /// <summary>
    /// Adds a new memory
    /// </summary>
    /// <param name="memory">The memory to add</param>
    /// <returns>The added memory</returns>
    Task<Memory> AddMemoryAsync(Memory memory);

    /// <summary>
    /// Gets a memory by ID and updates access tracking
    /// </summary>
    /// <param name="memoryId">ID of the memory to retrieve</param>
    /// <returns>The memory or null if not found</returns>
    Task<Memory?> GetMemoryByIdAsync(string memoryId);

    /// <summary>
    /// Gets all memories
    /// </summary>
    /// <returns>List of all memories</returns>
    Task<List<Memory>> GetAllMemoriesAsync();

    /// <summary>
    /// Updates an existing memory
    /// </summary>
    /// <param name="memory">The memory to update</param>
    /// <returns>The updated memory</returns>
    Task<Memory> UpdateMemoryAsync(Memory memory);

    /// <summary>
    /// Deletes a memory by ID
    /// </summary>
    /// <param name="memoryId">ID of the memory to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteMemoryAsync(string memoryId);

    /// <summary>
    /// Searches memories by content or tags
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <returns>List of matching memories</returns>
    Task<List<Memory>> SearchMemoriesAsync(string searchTerm);
}
