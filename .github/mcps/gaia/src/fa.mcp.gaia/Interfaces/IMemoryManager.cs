namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for memory management operations - MCP server tools
/// </summary>
public interface IMemoryManager
{
    /// <summary>
    /// Remembers something useful for the future via MCP
    /// </summary>
    /// <param name="title">Brief title or description of what's being remembered</param>
    /// <param name="content">The actual content to remember</param>
    /// <param name="tags">Comma-separated tags for categorization</param>
    /// <param name="priority">Priority level (1-5, 5 being highest)</param>
    /// <param name="createdBy">Who or what is creating this memory</param>
    /// <returns>JSON string containing the created memory with its ID</returns>
    Task<string> RememberAsync(string title, string content, string tags, int priority, string createdBy);

    /// <summary>
    /// Recalls a memory by its ID via MCP
    /// </summary>
    /// <param name="memoryId">ID of the memory to recall</param>
    /// <returns>JSON string containing the memory contents</returns>
    Task<string> RecallAsync(string memoryId);

    /// <summary>
    /// Lists all memories via MCP
    /// </summary>
    /// <param name="includeContent">Whether to include full content or just summaries</param>
    /// <returns>JSON string containing list of memories and their IDs</returns>
    Task<string> ListAllMemoriesAsync(bool includeContent);

    /// <summary>
    /// Searches memories by content, title, or tags via MCP
    /// </summary>
    /// <param name="searchTerm">Term to search for in title, content, or tags</param>
    /// <param name="includeContent">Whether to include full content or just summaries</param>
    /// <returns>JSON string containing list of matching memories</returns>
    Task<string> SearchMemoriesAsync(string searchTerm, bool includeContent);
}
