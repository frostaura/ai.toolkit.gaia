using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace FrostAura.MCP.Gaia.Models
{
    /// <summary>
    /// Represents a per-project task store with TTL support
    /// Each project gets its own isolated task storage to prevent cross-project interference
    /// </summary>
    [Description("A project-scoped task store with time-to-live support")]
    public class ProjectTaskStore
    {
        /// <summary>
        /// The sanitized project name this store belongs to
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// Thread-safe storage for this project's tasks
        /// Key is the task ID (not prefixed with project name since it's already scoped)
        /// </summary>
        public ConcurrentDictionary<string, GaiaTask> Tasks { get; } = new();

        /// <summary>
        /// UTC timestamp when this project store was created
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// UTC timestamp when this project store was last accessed (read or write)
        /// Used for TTL expiration
        /// </summary>
        public DateTime LastAccessed { get; private set; }

        /// <summary>
        /// Creates a new project task store
        /// </summary>
        /// <param name="projectName">The sanitized project name</param>
        public ProjectTaskStore(string projectName)
        {
            ProjectName = projectName;
            Created = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the last accessed timestamp to now
        /// Call this on any read or write operation
        /// </summary>
        public void Touch()
        {
            LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if this store has exceeded the given TTL
        /// </summary>
        /// <param name="ttl">The time-to-live duration</param>
        /// <returns>True if the store is expired, false otherwise</returns>
        public bool IsExpired(TimeSpan ttl)
        {
            return DateTime.UtcNow - LastAccessed > ttl;
        }
    }
}
