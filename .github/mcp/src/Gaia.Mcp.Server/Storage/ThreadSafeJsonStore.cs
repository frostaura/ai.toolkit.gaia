using System.Collections.Concurrent;
using System.Text.Json;

namespace Gaia.Mcp.Server.Storage;

/// <summary>
/// Generic thread-safe JSON file store. Uses a SemaphoreSlim per key to serialize
/// reads/writes to the same file while allowing concurrent access to different files.
/// </summary>
public sealed class ThreadSafeJsonStore<T>
{
    private static readonly JsonSerializerOptions s_options = new() { WriteIndented = true };

    private readonly string _root;
    private readonly string _suffix;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    /// <param name="rootDir">Directory where JSON files are stored.</param>
    /// <param name="fileSuffix">File suffix including dot, e.g. ".tasks.json".</param>
    public ThreadSafeJsonStore(string rootDir, string fileSuffix)
    {
        _root = rootDir;
        _suffix = fileSuffix;
        Directory.CreateDirectory(_root);
    }

    /// <summary>
    /// Sanitize key to prevent path traversal. Only alphanumeric, dash, underscore, and dot are allowed.
    /// </summary>
    internal static string SanitizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key must not be null or empty.", nameof(key));

        // Replace any character that isn't alphanumeric, dash, underscore, or dot with underscore.
        var sanitized = new string(key.Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.' ? c : '_').ToArray());

        // Prevent directory traversal patterns like ".." even after sanitization.
        sanitized = sanitized.Replace("..", "_");

        if (string.IsNullOrWhiteSpace(sanitized))
            throw new ArgumentException("Key resolves to empty after sanitization.", nameof(key));

        return sanitized;
    }

    private string PathFor(string key) => Path.Combine(_root, $"{SanitizeKey(key)}{_suffix}");

    private SemaphoreSlim LockFor(string key) =>
        _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

    public async Task<List<T>> LoadAsync(string key)
    {
        var sem = LockFor(key);
        await sem.WaitAsync();
        try
        {
            var path = PathFor(key);
            if (!File.Exists(path)) return new();
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<T>>(json, s_options) ?? new();
        }
        finally
        {
            sem.Release();
        }
    }

    public async Task SaveAsync(string key, List<T> items)
    {
        var sem = LockFor(key);
        await sem.WaitAsync();
        try
        {
            var path = PathFor(key);
            var json = JsonSerializer.Serialize(items, s_options);
            // Write to temp then move for atomicity.
            var tmp = path + ".tmp";
            await File.WriteAllTextAsync(tmp, json);
            File.Move(tmp, path, overwrite: true);
        }
        finally
        {
            sem.Release();
        }
    }

    /// <summary>
    /// Read-modify-write under a single lock acquisition.
    /// </summary>
    public async Task<List<T>> MutateAsync(string key, Action<List<T>> mutate)
    {
        var sem = LockFor(key);
        await sem.WaitAsync();
        try
        {
            var path = PathFor(key);
            List<T> items;
            if (File.Exists(path))
            {
                var json = await File.ReadAllTextAsync(path);
                items = JsonSerializer.Deserialize<List<T>>(json, s_options) ?? new();
            }
            else
            {
                items = new();
            }

            mutate(items);

            var outJson = JsonSerializer.Serialize(items, s_options);
            var tmp = path + ".tmp";
            await File.WriteAllTextAsync(tmp, outJson);
            File.Move(tmp, path, overwrite: true);

            return items;
        }
        finally
        {
            sem.Release();
        }
    }
}
