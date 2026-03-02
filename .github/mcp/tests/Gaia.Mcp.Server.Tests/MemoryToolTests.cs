using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Tools;

namespace Gaia.Mcp.Server.Tests;

public class MemoryToolTests : IDisposable
{
    private readonly string _tempDir;
    private readonly MemoryTool _tool;

    public MemoryToolTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _tool = new MemoryTool(new ThreadSafeJsonStore<MemoryItem>(_tempDir, ".memory.json"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task Remember_CreatesNewItem()
    {
        var item = await _tool.Remember("proj", "build/command", "dotnet build");

        Assert.Equal("build/command", item.Key);
        Assert.Equal("dotnet build", item.Value);
        Assert.Equal("proj", item.Project);
    }

    [Fact]
    public async Task Remember_UpsertsExistingKey()
    {
        await _tool.Remember("proj", "build/command", "dotnet build");
        var updated = await _tool.Remember("proj", "build/command", "make build");

        Assert.Equal("make build", updated.Value);

        var all = await _tool.Recall("proj");
        Assert.Single(all);
    }

    [Fact]
    public async Task Remember_UpdatesTimestamp_OnUpsert()
    {
        var original = await _tool.Remember("proj", "key", "value1");
        var originalUpdated = original.UpdatedUtc;

        await Task.Delay(10); // ensure time difference
        var updated = await _tool.Remember("proj", "key", "value2");

        Assert.True(updated.UpdatedUtc >= originalUpdated);
    }

    [Fact]
    public async Task Recall_ReturnsAllItems()
    {
        await _tool.Remember("proj", "key1", "val1");
        await _tool.Remember("proj", "key2", "val2");

        var all = await _tool.Recall("proj");
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task Recall_FiltersByKeyPrefix()
    {
        await _tool.Remember("proj", "build/command", "dotnet build");
        await _tool.Remember("proj", "build/args", "--release");
        await _tool.Remember("proj", "env/DB_URL", "localhost");

        var buildItems = await _tool.Recall("proj", "build/");
        Assert.Equal(2, buildItems.Count);
        Assert.All(buildItems, item => Assert.StartsWith("build/", item.Key));
    }

    [Fact]
    public async Task Recall_KeyPrefixIsCaseInsensitive()
    {
        await _tool.Remember("proj", "Build/Command", "dotnet build");

        var items = await _tool.Recall("proj", "build/");
        Assert.Single(items);
    }

    [Fact]
    public async Task Recall_EmptyProject_ReturnsEmpty()
    {
        var items = await _tool.Recall("nonexistent");
        Assert.Empty(items);
    }

    [Fact]
    public async Task Forget_RemovesItem()
    {
        await _tool.Remember("proj", "key1", "val1");
        var result = await _tool.Forget("proj", "key1");

        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());

        var all = await _tool.Recall("proj");
        Assert.Empty(all);
    }

    [Fact]
    public async Task Forget_NonExistentKey_ReturnsFalse()
    {
        var result = await _tool.Forget("proj", "missing-key");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task Clear_RemovesAllItems()
    {
        await _tool.Remember("proj", "key1", "val1");
        await _tool.Remember("proj", "key2", "val2");

        await _tool.Clear("proj");

        var all = await _tool.Recall("proj");
        Assert.Empty(all);
    }
}
