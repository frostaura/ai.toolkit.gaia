using Gaia.Workflows.Server.Tools;

namespace Gaia.Workflows.Server.Tests;

public class WorkflowsToolTests : IDisposable
{
    private readonly string _tempDir;
    private readonly WorkflowsTool _tool;

    public WorkflowsToolTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"gaia-wf-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _tool = new WorkflowsTool(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private void WriteWorkflow(string filename, string content)
    {
        File.WriteAllText(Path.Combine(_tempDir, filename), content);
    }

    [Fact]
    public void ListWorkflows_ReturnsAvailableWorkflows()
    {
        WriteWorkflow("hello.yml", @"
name: hello
description: Say hello
params:
  - name: name
    description: Who to greet
steps:
  - id: greet
    run: echo Hello
");

        var workflows = _tool.ListWorkflows();
        Assert.Single(workflows);
        Assert.Equal("hello", workflows[0].Name);
    }

    [Fact]
    public void ListWorkflows_EmptyDir_ReturnsEmpty()
    {
        var workflows = _tool.ListWorkflows();
        Assert.Empty(workflows);
    }

    [Fact]
    public async Task ExecuteWorkflow_NonExistent_ReturnsError()
    {
        var result = await _tool.ExecuteWorkflow("nonexistent");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task ExecuteWorkflow_NoSteps_ReturnsError()
    {
        WriteWorkflow("empty.yml", @"
name: empty
description: No steps workflow
steps: []
");

        var result = await _tool.ExecuteWorkflow("empty");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task ExecuteWorkflow_InvalidArgsJson_ReturnsError()
    {
        WriteWorkflow("test.yml", @"
name: test
description: Test workflow
steps:
  - id: s1
    run: echo hi
");

        var result = await _tool.ExecuteWorkflow("test", args: "not-valid-json");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task ExecuteWorkflow_SimpleStep_ReturnsSuccess()
    {
        WriteWorkflow("simple.yml", @"
name: simple
description: Simple workflow
steps:
  - id: s1
    run: echo hello-world
");

        var result = await _tool.ExecuteWorkflow("simple");
        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());
        Assert.Equal(0, json.GetProperty("exitCode").GetInt32());
        Assert.Contains("hello-world", json.GetProperty("output").GetString());
    }

    [Fact]
    public async Task ExecuteWorkflow_ParamsSubstitution()
    {
        WriteWorkflow("params.yml", @"
name: params
description: Params test
params:
  - name: greeting
    description: Greeting text
steps:
  - id: s1
    run: echo ${{ params.greeting }}
");

        var result = await _tool.ExecuteWorkflow("params", args: "{\"greeting\":\"hi-there\"}");
        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());
        Assert.Contains("hi-there", json.GetProperty("output").GetString());
    }

    [Fact]
    public async Task ExecuteWorkflow_StepOutputSubstitution()
    {
        WriteWorkflow("chain.yml", @"
name: chain
description: Chained steps
steps:
  - id: first
    run: echo step-one-output
  - id: second
    run: echo ""received ${{ steps.first.output }}""
");

        var result = await _tool.ExecuteWorkflow("chain");
        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());
        var output = json.GetProperty("output").GetString()!;
        Assert.Contains("step-one-output", output);
        Assert.Contains("received step-one-output", output);
    }

    [Fact]
    public async Task ExecuteWorkflow_EnvVarsFromArgs()
    {
        WriteWorkflow("envvar.yml", @"
name: envvar
description: Env vars test
params:
  - name: MY_VAR
    description: Test var
steps:
  - id: s1
    run: echo $MY_VAR
");

        var result = await _tool.ExecuteWorkflow("envvar", args: "{\"MY_VAR\":\"env-value\"}");
        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());
        Assert.Contains("env-value", json.GetProperty("output").GetString());
    }

    [Fact]
    public async Task ExecuteWorkflow_FailedStep_ReturnsError()
    {
        WriteWorkflow("fail.yml", @"
name: fail
description: Failing workflow
steps:
  - id: good
    run: echo ok
  - id: bad
    run: exit 1
");

        var result = await _tool.ExecuteWorkflow("fail");
        var json = TestHelpers.ToJson(result);
        Assert.False(json.GetProperty("ok").GetBoolean());
        Assert.NotEqual(0, json.GetProperty("exitCode").GetInt32());
        Assert.Equal("bad", json.GetProperty("failedStep").GetString());
    }

    [Fact]
    public async Task ExecuteWorkflow_FinalOutput_IsLastStepOutput()
    {
        WriteWorkflow("multi.yml", @"
name: multi
description: Multi step
steps:
  - id: s1
    run: echo first
  - id: s2
    run: echo second
  - id: s3
    run: echo final-output
");

        var result = await _tool.ExecuteWorkflow("multi");
        var json = TestHelpers.ToJson(result);
        Assert.True(json.GetProperty("ok").GetBoolean());
        Assert.Equal("final-output", json.GetProperty("finalOutput").GetString());
    }
}
