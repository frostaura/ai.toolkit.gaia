using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Validation;

namespace Gaia.Mcp.Server.Tests;

public class CompletionValidatorTests
{
    private static TaskItem MakeValidTask() => new()
    {
        Id = "abc123",
        Project = "test",
        Title = "Test task",
        Status = "doing",
        RequiredGates = new List<string> { "lint", "build" },
        GatesSatisfied = new List<string> { "lint", "build" },
        Proof = new ProofArgs
        {
            ChangedFiles = new List<string> { "src/File.cs" },
            TestsAdded = new List<string> { "tests/FileTests.cs" },
            ManualRegression = new List<string> { "curl" }
        }
    };

    [Fact]
    public void ValidTask_ReturnsNull()
    {
        var task = MakeValidTask();
        var result = CompletionValidator.ValidateMarkDone(task);
        Assert.Null(result);
    }

    [Fact]
    public void NeedsInputBlockers_ReturnsNeedsInputError()
    {
        var task = MakeValidTask();
        task.Blockers.Add("NEEDS_INPUT: Is this a breaking change?");

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.NeedsInputUnresolved, result.Code);
        Assert.Contains("NEEDS_INPUT", result.Message);
    }

    [Fact]
    public void GenericBlockers_ReturnsBlockersUnresolvedError()
    {
        var task = MakeValidTask();
        task.Blockers.Add("Waiting on upstream dependency");

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.BlockersUnresolved, result.Code);
        Assert.Contains("blockers", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NeedsInputTakesPriorityOverGenericBlockers()
    {
        var task = MakeValidTask();
        task.Blockers.Add("Generic blocker");
        task.Blockers.Add("NEEDS_INPUT: Clarify scope");

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.NeedsInputUnresolved, result.Code);
    }

    [Fact]
    public void EmptyChangedFiles_ReturnsMissingProofArgs()
    {
        var task = MakeValidTask();
        task.Proof.ChangedFiles.Clear();

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.MissingProofArgs, result.Code);
        Assert.Contains("changedFiles", result.Message);
    }

    [Fact]
    public void EmptyTestsAdded_ReturnsMissingProofArgs()
    {
        var task = MakeValidTask();
        task.Proof.TestsAdded.Clear();

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.MissingProofArgs, result.Code);
        Assert.Contains("testsAdded", result.Message);
    }

    [Fact]
    public void EmptyManualRegression_ReturnsMissingProofArgs()
    {
        var task = MakeValidTask();
        task.Proof.ManualRegression.Clear();

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.MissingProofArgs, result.Code);
        Assert.Contains("manualRegressionLabels", result.Message);
    }

    [Fact]
    public void MultipleEmptyProofArrays_ListsAllMissing()
    {
        var task = MakeValidTask();
        task.Proof.ChangedFiles.Clear();
        task.Proof.TestsAdded.Clear();
        task.Proof.ManualRegression.Clear();

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.MissingProofArgs, result.Code);
        Assert.Contains("changedFiles", result.Message);
        Assert.Contains("testsAdded", result.Message);
        Assert.Contains("manualRegressionLabels", result.Message);
    }

    [Fact]
    public void UnsatisfiedGates_ReturnsGatesUnsatisfied()
    {
        var task = MakeValidTask();
        task.GatesSatisfied = new List<string> { "lint" }; // missing "build"

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.GatesUnsatisfied, result.Code);
        Assert.Contains("build", result.Message);
    }

    [Fact]
    public void NoRequiredGates_PassesGateCheck()
    {
        var task = MakeValidTask();
        task.RequiredGates.Clear();
        task.GatesSatisfied.Clear();

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.Null(result);
    }

    [Fact]
    public void NeedsInputCaseInsensitive()
    {
        var task = MakeValidTask();
        task.Blockers.Add("needs_input: lowercase question");

        var result = CompletionValidator.ValidateMarkDone(task);

        Assert.NotNull(result);
        Assert.Equal(ErrorCodes.NeedsInputUnresolved, result.Code);
    }
}
