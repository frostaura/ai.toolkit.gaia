using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Tools;

namespace Gaia.Mcp.Server.Tests;

public class TasksToolTests : IDisposable
{
    private readonly string _tempDir;
    private readonly TasksTool _tool;

    public TasksToolTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _tool = new TasksTool(new JsonTaskStore(_tempDir));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task Create_GeneratesUniqueHexId()
    {
        var task = await _tool.Create("proj", "My task");

        Assert.NotNull(task.Id);
        Assert.Equal(32, task.Id.Length);
        Assert.True(task.Id.All(c => "0123456789abcdef".Contains(c)));
    }

    [Fact]
    public async Task Create_SetsStatusToTodo()
    {
        var task = await _tool.Create("proj", "Task");
        Assert.Equal("todo", task.Status);
    }

    [Fact]
    public async Task Create_StoresDescriptionAndGates()
    {
        var task = await _tool.Create("proj", "Task", "My description", new[] { "lint", "build" });

        Assert.Equal("My description", task.Description);
        Assert.Equal(new[] { "lint", "build" }, task.RequiredGates);
    }

    [Fact]
    public async Task Create_DefaultsToEmptyGates()
    {
        var task = await _tool.Create("proj", "Task");
        Assert.Empty(task.RequiredGates);
    }

    [Fact]
    public async Task Create_PersistsTask()
    {
        await _tool.Create("proj", "Task 1");
        var list = await _tool.List("proj");
        Assert.Single(list);
    }

    [Fact]
    public async Task Create_MultipleTasksGetUniqueIds()
    {
        var t1 = await _tool.Create("proj", "Task 1");
        var t2 = await _tool.Create("proj", "Task 2");
        Assert.NotEqual(t1.Id, t2.Id);
    }

    [Fact]
    public async Task List_EmptyProject_ReturnsEmpty()
    {
        var list = await _tool.List("nonexistent");
        Assert.Empty(list);
    }

    [Fact]
    public async Task List_ReturnsAllTasks()
    {
        await _tool.Create("proj", "Task 1");
        await _tool.Create("proj", "Task 2");
        await _tool.Create("proj", "Task 3");

        var list = await _tool.List("proj");
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public async Task Update_ChangesTitle()
    {
        var task = await _tool.Create("proj", "Original");
        var updated = await _tool.Update("proj", task.Id, title: "Updated");

        Assert.Equal("Updated", updated.Title);
    }

    [Fact]
    public async Task Update_ChangesDescription()
    {
        var task = await _tool.Create("proj", "Task", "Old desc");
        var updated = await _tool.Update("proj", task.Id, description: "New desc");

        Assert.Equal("New desc", updated.Description);
    }

    [Fact]
    public async Task Update_ChangesStatus()
    {
        var task = await _tool.Create("proj", "Task");
        var updated = await _tool.Update("proj", task.Id, status: "doing");

        Assert.Equal("doing", updated.Status);
    }

    [Fact]
    public async Task Update_ReplacesRequiredGates()
    {
        var task = await _tool.Create("proj", "Task", requiredGates: new[] { "lint" });
        var updated = await _tool.Update("proj", task.Id, requiredGates: new[] { "lint", "build", "ci" });

        Assert.Equal(new[] { "lint", "build", "ci" }, updated.RequiredGates);
    }

    [Fact]
    public async Task Update_ReplacesGatesSatisfied()
    {
        var task = await _tool.Create("proj", "Task", requiredGates: new[] { "lint", "build" });
        var updated = await _tool.Update("proj", task.Id, gatesSatisfied: new[] { "lint" });

        Assert.Equal(new[] { "lint" }, updated.GatesSatisfied);
    }

    [Fact]
    public async Task Update_ClearsBlockersWithEmptyArray()
    {
        var task = await _tool.Create("proj", "Task");
        await _tool.FlagNeedsInput("proj", task.Id, new[] { "Question?" });
        var updated = await _tool.Update("proj", task.Id, blockers: Array.Empty<string>());

        Assert.Empty(updated.Blockers);
    }

    [Fact]
    public async Task Update_LeavesFieldsUnchangedWhenNull()
    {
        var task = await _tool.Create("proj", "Original Title", "Original Desc",
            new[] { "lint" });

        var updated = await _tool.Update("proj", task.Id);

        Assert.Equal("Original Title", updated.Title);
        Assert.Equal("Original Desc", updated.Description);
        Assert.Equal("todo", updated.Status);
        Assert.Equal(new[] { "lint" }, updated.RequiredGates);
    }

    [Fact]
    public async Task MarkDone_SetsStatusToDone_WhenValid()
    {
        var task = await _tool.Create("proj", "Task");
        var result = await _tool.MarkDone("proj", task.Id,
            new[] { "src/File.cs" },
            new[] { "tests/FileTests.cs" },
            new[] { "curl" });

        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());

        var list = await _tool.List("proj");
        Assert.Equal("done", list[0].Status);
    }

    [Fact]
    public async Task MarkDone_ReturnsError_WhenProofMissing()
    {
        var task = await _tool.Create("proj", "Task");
        var result = await _tool.MarkDone("proj", task.Id,
            Array.Empty<string>(),
            new[] { "tests/FileTests.cs" },
            new[] { "curl" });

        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task MarkDone_RevertsProof_OnFailure()
    {
        var task = await _tool.Create("proj", "Task", requiredGates: new[] { "lint" });
        // Gates not satisfied -> should fail
        await _tool.MarkDone("proj", task.Id,
            new[] { "src/File.cs" },
            new[] { "tests/FileTests.cs" },
            new[] { "curl" });

        var list = await _tool.List("proj");
        Assert.Empty(list[0].Proof.ChangedFiles); // Proof should be reverted
        Assert.NotEqual("done", list[0].Status);
    }

    [Fact]
    public async Task FlagNeedsInput_AppendsBlockers()
    {
        var task = await _tool.Create("proj", "Task");
        var updated = await _tool.FlagNeedsInput("proj", task.Id,
            new[] { "Is this breaking?" });

        Assert.Single(updated.Blockers);
        Assert.Equal("NEEDS_INPUT: Is this breaking?", updated.Blockers[0]);
    }

    [Fact]
    public async Task FlagNeedsInput_SupportsMultipleQuestions()
    {
        var task = await _tool.Create("proj", "Task");
        var updated = await _tool.FlagNeedsInput("proj", task.Id,
            new[] { "Q1?", "Q2?" });

        Assert.Equal(2, updated.Blockers.Count);
        Assert.Equal("NEEDS_INPUT: Q1?", updated.Blockers[0]);
        Assert.Equal("NEEDS_INPUT: Q2?", updated.Blockers[1]);
    }

    [Fact]
    public async Task Delete_RemovesTask()
    {
        var task = await _tool.Create("proj", "Task");
        var result = await _tool.Delete("proj", task.Id);

        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());

        var list = await _tool.List("proj");
        Assert.Empty(list);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_ForNonExistent()
    {
        var result = await _tool.Delete("proj", "nonexistent");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task ClearAll_RemovesAllTasks()
    {
        await _tool.Create("proj", "Task 1");
        await _tool.Create("proj", "Task 2");

        await _tool.ClearAll("proj");

        var list = await _tool.List("proj");
        Assert.Empty(list);
    }

    [Fact]
    public async Task MarkDone_WithGates_Succeeds()
    {
        var task = await _tool.Create("proj", "Task", requiredGates: new[] { "lint", "build" });
        await _tool.Update("proj", task.Id, gatesSatisfied: new[] { "lint", "build" });

        var result = await _tool.MarkDone("proj", task.Id,
            new[] { "src/File.cs" },
            new[] { "tests/FileTests.cs" },
            new[] { "curl" });

        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task MarkDone_WithUnsatisfiedGates_Fails()
    {
        var task = await _tool.Create("proj", "Task", requiredGates: new[] { "lint", "build" });
        await _tool.Update("proj", task.Id, gatesSatisfied: new[] { "lint" });

        var result = await _tool.MarkDone("proj", task.Id,
            new[] { "src/File.cs" },
            new[] { "tests/FileTests.cs" },
            new[] { "curl" });

        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task MarkDone_WithBlockers_Fails()
    {
        var task = await _tool.Create("proj", "Task");
        await _tool.FlagNeedsInput("proj", task.Id, new[] { "Question?" });

        var result = await _tool.MarkDone("proj", task.Id,
            new[] { "src/File.cs" },
            new[] { "tests/FileTests.cs" },
            new[] { "curl" });

        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }
}
