using System.ComponentModel;
using System.Text.Json;
using FrostAura.MCP.Gaia.Configuration;
using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for memory operations with integrated MCP tools
/// </summary>
[McpServerToolType]
public class MemoryManager : IMemoryManager
{
    private readonly IMemoryRepository _repository;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the MemoryManager
    /// </summary>
    /// <param name="repository">Memory repository</param>
    /// <param name="configuration">Configuration to check for error handling preferences</param>
    public MemoryManager(IMemoryRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _jsonOptions = JsonConfiguration.GetApiOptions();
    }

    /// <summary>
    /// Remembers something useful for the future via MCP
    /// </summary>
    /// <param name="title">Brief title or description of what's being remembered</param>
    /// <param name="content">The actual content to remember</param>
    /// <param name="tags">Comma-separated tags for categorization</param>
    /// <param name="priority">Priority level (1-5, 5 being highest)</param>
    /// <param name="createdBy">Who or what is creating this memory</param>
    /// <returns>JSON string containing the created memory with its ID</returns>
    [McpServerTool]
    [Description("Allows for remembering something useful for the future. Stores information that can be recalled later by ID or searched through.")]
    public async Task<string> RememberAsync(
        [Description("Brief title or description of what's being remembered")] string title,
        [Description("The actual content to remember - can be detailed information, code snippets, decisions, etc.")] string content,
        [Description("Comma-separated tags for categorization (e.g., 'decision,architecture,api')")] string tags = "",
        [Description("Priority level (1-5, 5 being highest)")] int priority = 3,
        [Description("Who or what is creating this memory (e.g., 'AI Assistant', 'User', 'System')")] string createdBy = "AI Assistant")
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));
        if (priority < 1 || priority > 5)
            throw new ArgumentException("Priority must be between 1 and 5.", nameof(priority));

        var tagList = string.IsNullOrWhiteSpace(tags)
            ? new List<string>()
            : tags.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();

        var memory = new Memory
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Content = content,
            Tags = tagList,
            Priority = priority,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddMemoryAsync(memory);

        var result = new
        {
            message = "Memory created successfully",
            memory = new
            {
                id = memory.Id,
                title = memory.Title,
                content = "obfuscated for token efficiency",
                tags = memory.Tags,
                priority = memory.Priority,
                createdBy = memory.CreatedBy,
                createdAt = memory.CreatedAt
            }
        };

        return JsonSerializer.Serialize(result, _jsonOptions);
    }

    /// <summary>
    /// Recalls a memory by its ID via MCP
    /// </summary>
    /// <param name="memoryId">ID of the memory to recall</param>
    /// <returns>JSON string containing the memory contents</returns>
    [McpServerTool]
    [Description("Given a memory ID, retrieves the complete memory contents including access tracking information.")]
    public async Task<string> RecallAsync(
        [Description("ID of the memory to recall")] string memoryId)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(memoryId))
            throw new ArgumentException("Memory ID cannot be null or empty.", nameof(memoryId));

        var memory = await _repository.GetMemoryByIdAsync(memoryId);

        if (memory == null)
        {
            var notFoundResult = new
            {
                message = "Memory not found",
                memoryId,
                memory = (Memory?)null
            };
            return JsonSerializer.Serialize(notFoundResult, _jsonOptions);
        }

        var result = new
        {
            message = "Memory recalled successfully",
            memoryId,
            memory = new
            {
                id = memory.Id,
                title = memory.Title,
                content = memory.Content,
                tags = memory.Tags,
                priority = memory.Priority,
                createdBy = memory.CreatedBy,
                createdAt = memory.CreatedAt,
                lastAccessedAt = memory.LastAccessedAt,
                accessCount = memory.AccessCount
            }
        };

        return JsonSerializer.Serialize(result, _jsonOptions);
    }

    /// <summary>
    /// Lists all memories via MCP
    /// </summary>
    /// <param name="includeContent">Whether to include full content or just summaries</param>
    /// <returns>JSON string containing list of memories and their IDs</returns>
    [McpServerTool]
    [Description("Returns a list of all memories with their IDs, titles, and metadata. Can optionally include full content.")]
    public async Task<string> ListAllMemoriesAsync(
        [Description("Whether to include full content or just summaries (title, tags, metadata)")] bool includeContent = false)
    {
        var memories = await _repository.GetAllMemoriesAsync();

        var memorySummaries = memories.Select(memory => new
        {
            id = memory.Id,
            title = memory.Title,
            content = includeContent ? memory.Content : $"{memory.Content.Substring(0, Math.Min(100, memory.Content.Length))}{(memory.Content.Length > 100 ? "..." : "")}",
            tags = memory.Tags,
            priority = memory.Priority,
            createdBy = memory.CreatedBy,
            createdAt = memory.CreatedAt,
            lastAccessedAt = memory.LastAccessedAt,
            accessCount = memory.AccessCount
        }).ToList();

        var result = new
        {
            message = "Memories retrieved successfully",
            totalCount = memories.Count,
            memories = memorySummaries
        };

        return JsonSerializer.Serialize(result, _jsonOptions);
    }

    /// <summary>
    /// Searches memories by content, title, or tags via MCP
    /// </summary>
    /// <param name="searchTerm">Term to search for in title, content, or tags</param>
    /// <param name="includeContent">Whether to include full content or just summaries</param>
    /// <returns>JSON string containing list of matching memories</returns>
    [McpServerTool]
    [Description("Searches memories by content, title, or tags. Returns matching memories sorted by priority and relevance.")]
    public async Task<string> SearchMemoriesAsync(
        [Description("Term to search for in title, content, or tags")] string searchTerm,
        [Description("Whether to include full content or just summaries (first 100 characters)")] bool includeContent = false)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be null or empty.", nameof(searchTerm));

        var memories = await _repository.SearchMemoriesAsync(searchTerm);

        var memorySummaries = memories.Select(memory => new
        {
            id = memory.Id,
            title = memory.Title,
            content = includeContent ? memory.Content : $"{memory.Content.Substring(0, Math.Min(100, memory.Content.Length))}{(memory.Content.Length > 100 ? "..." : "")}",
            tags = memory.Tags,
            priority = memory.Priority,
            createdBy = memory.CreatedBy,
            createdAt = memory.CreatedAt,
            lastAccessedAt = memory.LastAccessedAt,
            accessCount = memory.AccessCount
        }).ToList();

        var result = new
        {
            message = $"Search completed for '{searchTerm}'",
            searchTerm,
            totalResults = memories.Count,
            memories = memorySummaries
        };

        return JsonSerializer.Serialize(result, _jsonOptions);
    }
}
