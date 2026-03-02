using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;

namespace Gaia.Mcp.Server.Tests;

public class ThreadSafeJsonStoreTests : IDisposable
{
    private readonly string _tempDir;

    public ThreadSafeJsonStoreTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task LoadAsync_NonExistentFile_ReturnsEmptyList()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");
        var result = await store.LoadAsync("missing-project");
        Assert.Empty(result);
    }

    [Fact]
    public async Task SaveAndLoad_RoundTrips()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");
        var items = new List<MemoryItem>
        {
            new() { Key = "k1", Value = "v1", Project = "p1" },
            new() { Key = "k2", Value = "v2", Project = "p1" }
        };

        await store.SaveAsync("p1", items);
        var loaded = await store.LoadAsync("p1");

        Assert.Equal(2, loaded.Count);
        Assert.Equal("k1", loaded[0].Key);
        Assert.Equal("v1", loaded[0].Value);
        Assert.Equal("k2", loaded[1].Key);
    }

    [Fact]
    public async Task MutateAsync_AppliesMutation()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");

        var result = await store.MutateAsync("proj", items =>
        {
            items.Add(new MemoryItem { Key = "a", Value = "1", Project = "proj" });
        });

        Assert.Single(result);
        Assert.Equal("a", result[0].Key);

        var loaded = await store.LoadAsync("proj");
        Assert.Single(loaded);
    }

    [Fact]
    public async Task MutateAsync_AtomicWriteDoesNotCorrupt()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");

        await store.MutateAsync("proj", items =>
        {
            items.Add(new MemoryItem { Key = "init", Value = "val", Project = "proj" });
        });

        // Multiple sequential mutations
        for (int i = 0; i < 10; i++)
        {
            await store.MutateAsync("proj", items =>
            {
                items.Add(new MemoryItem { Key = $"key-{i}", Value = $"val-{i}", Project = "proj" });
            });
        }

        var loaded = await store.LoadAsync("proj");
        Assert.Equal(11, loaded.Count); // 1 initial + 10 added
    }

    [Fact]
    public async Task ConcurrentAccess_DifferentKeys_NoCorruption()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");

        var tasks = Enumerable.Range(0, 10).Select(i =>
            store.MutateAsync($"project-{i}", items =>
            {
                items.Add(new MemoryItem { Key = "key", Value = $"val-{i}", Project = $"project-{i}" });
            })
        ).ToArray();

        await Task.WhenAll(tasks);

        for (int i = 0; i < 10; i++)
        {
            var loaded = await store.LoadAsync($"project-{i}");
            Assert.Single(loaded);
            Assert.Equal($"val-{i}", loaded[0].Value);
        }
    }

    [Fact]
    public async Task ConcurrentAccess_SameKey_NoDataLoss()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");

        var tasks = Enumerable.Range(0, 20).Select(i =>
            store.MutateAsync("shared", items =>
            {
                items.Add(new MemoryItem { Key = $"k-{i}", Value = $"v-{i}", Project = "shared" });
            })
        ).ToArray();

        await Task.WhenAll(tasks);

        var loaded = await store.LoadAsync("shared");
        Assert.Equal(20, loaded.Count);
    }

    [Fact]
    public async Task SaveAsync_OverwritesExistingData()
    {
        var store = new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".test.json");

        await store.SaveAsync("proj", new List<MemoryItem>
        {
            new() { Key = "old", Value = "old-val", Project = "proj" }
        });

        await store.SaveAsync("proj", new List<MemoryItem>
        {
            new() { Key = "new", Value = "new-val", Project = "proj" }
        });

        var loaded = await store.LoadAsync("proj");
        Assert.Single(loaded);
        Assert.Equal("new", loaded[0].Key);
    }
}
