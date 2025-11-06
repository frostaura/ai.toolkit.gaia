using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// Repository for memory operations
/// </summary>
public class MemoryRepository : IMemoryRepository
{
    private readonly TaskPlannerDbContext _context;

    /// <summary>
    /// Initializes a new instance of the MemoryRepository
    /// </summary>
    /// <param name="context">Database context</param>
    public MemoryRepository(TaskPlannerDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new memory
    /// </summary>
    /// <param name="memory">The memory to add</param>
    /// <returns>The added memory</returns>
    public async Task<Memory> AddMemoryAsync(Memory memory)
    {
        if (await _context.Memories.AnyAsync(m => m.Id == memory.Id))
        {
            throw new InvalidOperationException($"A memory with ID '{memory.Id}' already exists.");
        }

        _context.Memories.Add(memory);
        await _context.SaveChangesAsync();
        return memory;
    }

    /// <summary>
    /// Gets a memory by ID and updates access tracking
    /// </summary>
    /// <param name="memoryId">ID of the memory to retrieve</param>
    /// <returns>The memory or null if not found</returns>
    public async Task<Memory?> GetMemoryByIdAsync(string memoryId)
    {
        var memory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == memoryId);

        if (memory != null)
        {
            // Update access tracking
            memory.LastAccessedAt = DateTime.UtcNow;
            memory.AccessCount++;
            await _context.SaveChangesAsync();
        }

        return memory;
    }

    /// <summary>
    /// Gets all memories
    /// </summary>
    /// <returns>List of all memories</returns>
    public async Task<List<Memory>> GetAllMemoriesAsync()
    {
        return await _context.Memories
            .OrderByDescending(m => m.Priority)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Updates an existing memory
    /// </summary>
    /// <param name="memory">The memory to update</param>
    /// <returns>The updated memory</returns>
    public async Task<Memory> UpdateMemoryAsync(Memory memory)
    {
        var existingMemory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == memory.Id);

        if (existingMemory == null)
        {
            throw new InvalidOperationException($"Memory with ID '{memory.Id}' not found.");
        }

        _context.Entry(existingMemory).CurrentValues.SetValues(memory);
        await _context.SaveChangesAsync();
        return memory;
    }

    /// <summary>
    /// Deletes a memory by ID
    /// </summary>
    /// <param name="memoryId">ID of the memory to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    public async Task<bool> DeleteMemoryAsync(string memoryId)
    {
        var memory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == memoryId);

        if (memory == null)
        {
            return false;
        }

        _context.Memories.Remove(memory);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Searches memories by content or tags
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <returns>List of matching memories</returns>
    public async Task<List<Memory>> SearchMemoriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllMemoriesAsync();
        }

        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        return await _context.Memories
            .Where(m =>
                m.Title.ToLower().Contains(lowerSearchTerm) ||
                m.Content.ToLower().Contains(lowerSearchTerm) ||
                m.TagsJson.ToLower().Contains(lowerSearchTerm))
            .OrderByDescending(m => m.Priority)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}
