using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Tools;

namespace Gaia.Mcp.Server.Tests;

public class SelfImproveToolTests : IDisposable
{
    private readonly string _tempDir;
    private readonly SelfImproveTool _tool;

    public SelfImproveToolTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _tool = new SelfImproveTool(new ThreadSafeJsonStore<ImprovementItem>(_tempDir, ".improvements.json"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task Log_CreatesItemWithUniqueId()
    {
        var item = await _tool.Log("proj", "Use lint gate always");

        Assert.NotNull(item.Id);
        Assert.Equal(32, item.Id.Length);
        Assert.Equal("proj", item.Project);
        Assert.Equal("Use lint gate always", item.Suggestion);
        Assert.False(item.Applied);
    }

    [Fact]
    public async Task Log_WithCategory()
    {
        var item = await _tool.Log("proj", "Fix CI", "ci");
        Assert.Equal("ci", item.Category);
    }

    [Fact]
    public async Task Log_WithoutCategory_IsNull()
    {
        var item = await _tool.Log("proj", "Suggestion");
        Assert.Null(item.Category);
    }

    [Fact]
    public async Task List_ReturnsAllItems()
    {
        await _tool.Log("proj1", "S1");
        await _tool.Log("proj2", "S2");

        var all = await _tool.List();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task List_FiltersByProject()
    {
        await _tool.Log("proj1", "S1");
        await _tool.Log("proj2", "S2");
        await _tool.Log("proj1", "S3");

        var filtered = await _tool.List(project: "proj1");
        Assert.Equal(2, filtered.Count);
        Assert.All(filtered, item => Assert.Equal("proj1", item.Project));
    }

    [Fact]
    public async Task List_FiltersByCategory()
    {
        await _tool.Log("proj", "S1", "ci");
        await _tool.Log("proj", "S2", "workflow");
        await _tool.Log("proj", "S3", "ci");

        var filtered = await _tool.List(category: "ci");
        Assert.Equal(2, filtered.Count);
        Assert.All(filtered, item => Assert.Equal("ci", item.Category));
    }

    [Fact]
    public async Task List_CategoryFilterIsCaseInsensitive()
    {
        await _tool.Log("proj", "S1", "CI");

        var filtered = await _tool.List(category: "ci");
        Assert.Single(filtered);
    }

    [Fact]
    public async Task List_FiltersByBothProjectAndCategory()
    {
        await _tool.Log("proj1", "S1", "ci");
        await _tool.Log("proj2", "S2", "ci");
        await _tool.Log("proj1", "S3", "workflow");

        var filtered = await _tool.List(project: "proj1", category: "ci");
        Assert.Single(filtered);
        Assert.Equal("S1", filtered[0].Suggestion);
    }

    [Fact]
    public async Task MarkApplied_SetsAppliedTrue()
    {
        var item = await _tool.Log("proj", "Suggestion");
        var result = await _tool.MarkApplied(item.Id);

        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());

        var all = await _tool.List();
        Assert.True(all[0].Applied);
    }

    [Fact]
    public async Task MarkApplied_NonExistent_ReturnsFalse()
    {
        var result = await _tool.MarkApplied("nonexistent");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task Clear_WithProject_RemovesOnlyThatProject()
    {
        await _tool.Log("proj1", "S1");
        await _tool.Log("proj2", "S2");

        await _tool.Clear("proj1");

        var all = await _tool.List();
        Assert.Single(all);
        Assert.Equal("proj2", all[0].Project);
    }

    [Fact]
    public async Task Clear_WithoutProject_RemovesAll()
    {
        await _tool.Log("proj1", "S1");
        await _tool.Log("proj2", "S2");

        await _tool.Clear();

        var all = await _tool.List();
        Assert.Empty(all);
    }
}
