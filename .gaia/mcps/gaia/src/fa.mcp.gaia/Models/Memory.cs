using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents a memory item for storing useful information
/// </summary>
public class Memory
{
    /// <summary>
    /// Unique identifier for the memory
    /// </summary>
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Title or brief description of what this memory contains
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The actual memory content - what needs to be remembered
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Tags to help categorize and search memories
    /// </summary>
    [NotMapped]
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Tags serialized as JSON for database storage
    /// </summary>
    [Column("Tags")]
    public string TagsJson
    {
        get => JsonSerializer.Serialize(Tags);
        set => Tags = string.IsNullOrEmpty(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
    }

    /// <summary>
    /// Who or what created this memory
    /// </summary>
    [MaxLength(200)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// When this memory was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this memory was last accessed/recalled
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// Number of times this memory has been accessed
    /// </summary>
    public int AccessCount { get; set; } = 0;

    /// <summary>
    /// Priority level of this memory (1-5, 5 being highest)
    /// </summary>
    [Range(1, 5)]
    public int Priority { get; set; } = 3;
}
