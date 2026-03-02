using Gaia.Workflows.Server.Parsing;

namespace Gaia.Workflows.Server.Tests;

public class WorkflowParserTests : IDisposable
{
    private readonly string _tempDir;

    public WorkflowParserTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-wf-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string WriteYaml(string filename, string content)
    {
        var path = Path.Combine(_tempDir, filename);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void Parse_NonExistentFile_ReturnsNull()
    {
        var result = WorkflowParser.Parse("/nonexistent/path.yml");
        Assert.Null(result);
    }

    [Fact]
    public void Parse_InvalidYaml_ReturnsNull()
    {
        var path = WriteYaml("invalid.yml", "this: is: not: valid: yaml: [[[");
        var result = WorkflowParser.Parse(path);
        Assert.Null(result);
    }

    [Fact]
    public void Parse_NoDescription_ReturnsNull()
    {
        var path = WriteYaml("no-desc.yml", @"
name: test-workflow
steps:
  - id: step1
    run: echo hello
");
        var result = WorkflowParser.Parse(path);
        Assert.Null(result);
    }

    [Fact]
    public void Parse_SetsNameFromFilename_WhenEmpty()
    {
        var path = WriteYaml("my-workflow.yml", @"
description: A test workflow
steps:
  - id: step1
    run: echo hello
");
        var result = WorkflowParser.Parse(path);

        Assert.NotNull(result);
        Assert.Equal("my-workflow", result.Name);
    }

    [Fact]
    public void Parse_UsesExplicitName()
    {
        var path = WriteYaml("file.yml", @"
name: explicit-name
description: A test workflow
steps:
  - id: step1
    run: echo hello
");
        var result = WorkflowParser.Parse(path);

        Assert.NotNull(result);
        Assert.Equal("explicit-name", result.Name);
    }

    [Fact]
    public void Parse_DeserializesFullWorkflow()
    {
        var path = WriteYaml("full.yml", @"
name: full-workflow
description: Full test workflow
params:
  - name: greeting
    description: The greeting to use
  - name: target
    description: The target name
output: Final greeting message
steps:
  - id: greet
    run: echo ${{ params.greeting }} ${{ params.target }}
  - id: done
    run: echo done
");
        var result = WorkflowParser.Parse(path);

        Assert.NotNull(result);
        Assert.Equal("full-workflow", result.Name);
        Assert.Equal("Full test workflow", result.Description);
        Assert.Equal(2, result.Params.Count);
        Assert.Equal("greeting", result.Params[0].Name);
        Assert.Equal("The greeting to use", result.Params[0].Description);
        Assert.Equal("target", result.Params[1].Name);
        Assert.Equal("Final greeting message", result.Output);
        Assert.Equal(2, result.Steps.Count);
        Assert.Equal("greet", result.Steps[0].Id);
        Assert.Equal("done", result.Steps[1].Id);
    }

    [Fact]
    public void Parse_SetsFilePath()
    {
        var path = WriteYaml("fp.yml", @"
description: test
steps:
  - id: s1
    run: echo hi
");
        var result = WorkflowParser.Parse(path);

        Assert.NotNull(result);
        Assert.Equal(path, result.FilePath);
    }

    [Fact]
    public void ScanDirectory_NonExistent_ReturnsEmpty()
    {
        var result = WorkflowParser.ScanDirectory("/nonexistent/dir");
        Assert.Empty(result);
    }

    [Fact]
    public void ScanDirectory_FindsAllYmlFiles()
    {
        WriteYaml("a.yml", "description: A\nsteps:\n  - id: s1\n    run: echo a");
        WriteYaml("b.yml", "description: B\nsteps:\n  - id: s1\n    run: echo b");
        WriteYaml("c.txt", "not a yaml file");

        var result = WorkflowParser.ScanDirectory(_tempDir);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ScanDirectory_SortsByName()
    {
        WriteYaml("z-workflow.yml", "description: Z\nsteps:\n  - id: s1\n    run: echo z");
        WriteYaml("a-workflow.yml", "description: A\nsteps:\n  - id: s1\n    run: echo a");

        var result = WorkflowParser.ScanDirectory(_tempDir);
        Assert.Equal("a-workflow", result[0].Name);
        Assert.Equal("z-workflow", result[1].Name);
    }

    [Fact]
    public void ScanDirectory_ExcludesInvalidWorkflows()
    {
        WriteYaml("valid.yml", "description: Valid\nsteps:\n  - id: s1\n    run: echo hi");
        WriteYaml("no-desc.yml", "name: no-desc\nsteps:\n  - id: s1\n    run: echo hi");

        var result = WorkflowParser.ScanDirectory(_tempDir);
        Assert.Single(result);
        Assert.Equal("valid", result[0].Name);
    }
}
