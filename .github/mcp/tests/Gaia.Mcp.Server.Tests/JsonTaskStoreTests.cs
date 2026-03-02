using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;

namespace Gaia.Mcp.Server.Tests;

public class JsonTaskStoreTests : IDisposable
{
    private readonly string _tempDir;
    private readonly JsonTaskStore _store;

    public JsonTaskStoreTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _store = new JsonTaskStore(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task LoadAsync_EmptyProject_ReturnsEmptyList()
    {
        var result = await _store.LoadAsync("empty-project");
        Assert.Empty(result);
    }

    [Fact]
    public async Task SaveAndLoad_PersistsTasks()
    {
        var tasks = new List<TaskItem>
        {
            new() { Id = "t1", Project = "proj", Title = "Task 1" },
            new() { Id = "t2", Project = "proj", Title = "Task 2" }
        };

        await _store.SaveAsync("proj", tasks);
        var loaded = await _store.LoadAsync("proj");

        Assert.Equal(2, loaded.Count);
        Assert.Equal("t1", loaded[0].Id);
        Assert.Equal("t2", loaded[1].Id);
    }

    [Fact]
    public async Task MutateAsync_AddAndPersist()
    {
        await _store.MutateAsync("proj", tasks =>
        {
            tasks.Add(new TaskItem { Id = "t1", Project = "proj", Title = "Added" });
        });

        var loaded = await _store.LoadAsync("proj");
        Assert.Single(loaded);
        Assert.Equal("Added", loaded[0].Title);
    }

    [Fact]
    public async Task UsesTasksJsonSuffix()
    {
        await _store.MutateAsync("myproject", tasks =>
        {
            tasks.Add(new TaskItem { Id = "t1", Project = "myproject", Title = "Test" });
        });

        var expectedFile = Path.Combine(_tempDir, "myproject.tasks.json");
        Assert.True(File.Exists(expectedFile));
    }
}
