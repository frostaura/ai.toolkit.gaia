using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Gaia.Workflows.Server.Models;
using Gaia.Workflows.Server.Parsing;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Gaia.Workflows.Server.Tools;

public sealed partial class WorkflowsTool
{
    private readonly string _workflowsDir;

    public WorkflowsTool(string workflowsDir)
    {
        _workflowsDir = workflowsDir;
    }

    [McpServerTool(Name = "workflows_list"), Description(
        "List all available Gaia workflows defined in .github/.agaia-workflows/. " +
        "Returns each workflow's name, description, parameters, and expected output. " +
        "Use this to discover what workflows are available before executing one.")]
    public List<WorkflowDescriptor> ListWorkflows()
    {
        return WorkflowParser.ScanDirectory(_workflowsDir);
    }

    [McpServerTool(Name = "workflows_execute"), Description(
        "Execute a Gaia workflow by name. The workflow is a YAML definition in .github/.agaia-workflows/. " +
        "Arguments are passed as a JSON object string where keys match param names from the workflow " +
        "header, and are set as environment variables for each step. Params are also available via " +
        "${{ params.<name> }} substitution in step commands. Each step's stdout is captured and " +
        "available to subsequent steps via ${{ steps.<id>.output }} substitution. " +
        "Output is streamed back via progress notifications as each step runs, " +
        "and the final result includes the full output and exit code.")]
    public async Task<object> ExecuteWorkflow(
        [Description("The name of the workflow to execute (without .yml extension). Use workflows_list to discover available names.")] string name,
        [Description("Optional JSON object string of arguments to pass to the workflow. Keys should match the @param names defined in the workflow header. Example: {\"name\": \"Dean\", \"greeting\": \"Hi\"}")] string? args = null,
        IProgress<ProgressNotificationValue>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var ymlPath = Path.Combine(_workflowsDir, $"{name}.yml");
        var descriptor = WorkflowParser.Parse(ymlPath);

        if (descriptor is null)
        {
            return new { ok = false, error = $"Workflow '{name}' not found or has no valid YAML definition at {ymlPath}" };
        }

        if (descriptor.Steps.Count == 0)
        {
            return new { ok = false, error = $"Workflow '{name}' has no steps defined." };
        }

        // Parse args JSON into env vars
        var envVars = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(args))
        {
            try
            {
                using var doc = JsonDocument.Parse(args);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    envVars[prop.Name] = prop.Value.ToString();
                }
            }
            catch (JsonException ex)
            {
                return new { ok = false, error = $"Invalid args JSON: {ex.Message}" };
            }
        }

        var workingDir = Path.GetFullPath(Path.Combine(_workflowsDir, "..", ".."));
        var stepOutputs = new Dictionary<string, string>();
        var allOutput = new StringBuilder();
        var totalSteps = descriptor.Steps.Count;

        for (var i = 0; i < totalSteps; i++)
        {
            var step = descriptor.Steps[i];
            var stepId = string.IsNullOrWhiteSpace(step.Id) ? $"step-{i}" : step.Id;

            progress?.Report(new ProgressNotificationValue
            {
                Progress = i,
                Total = totalSteps,
                Message = $"[step {i + 1}/{totalSteps}] Starting: {stepId}"
            });

            // Apply ${{ params.<name> }} and ${{ steps.<id>.output }} substitutions
            var command = ApplySubstitutions(step.Run, stepOutputs, envVars);

            var result = await RunStepAsync(command, envVars, workingDir, cancellationToken);

            stepOutputs[stepId] = result.Output;
            allOutput.AppendLine($"[{stepId}] {result.Output}");

            if (result.ExitCode != 0)
            {
                progress?.Report(new ProgressNotificationValue
                {
                    Progress = i + 1,
                    Total = totalSteps,
                    Message = $"[step {i + 1}/{totalSteps}] FAILED: {stepId} (exit code {result.ExitCode})"
                });

                if (!string.IsNullOrWhiteSpace(result.Stderr))
                {
                    allOutput.AppendLine($"[{stepId}:stderr] {result.Stderr}");
                }

                return new
                {
                    ok = false,
                    exitCode = result.ExitCode,
                    workflow = name,
                    failedStep = stepId,
                    output = allOutput.ToString().TrimEnd()
                };
            }

            progress?.Report(new ProgressNotificationValue
            {
                Progress = i + 1,
                Total = totalSteps,
                Message = $"[step {i + 1}/{totalSteps}] Completed: {stepId}"
            });
        }

        return new
        {
            ok = true,
            exitCode = 0,
            workflow = name,
            output = allOutput.ToString().TrimEnd()
        };
    }

    private static string ApplySubstitutions(string command, Dictionary<string, string> stepOutputs, Dictionary<string, string> envVars)
    {
        // Replace ${{ params.<name> }} with the corresponding input value
        var result = ParamsPattern().Replace(command, match =>
        {
            var paramName = match.Groups[1].Value;
            return envVars.TryGetValue(paramName, out var value) ? value : match.Value;
        });

        // Replace ${{ steps.<id>.output }} with the captured step output
        result = StepOutputPattern().Replace(result, match =>
        {
            var stepId = match.Groups[1].Value;
            return stepOutputs.TryGetValue(stepId, out var output) ? output : match.Value;
        });

        return result;
    }

    [GeneratedRegex(@"\$\{\{\s*params\.([a-zA-Z0-9_-]+)\s*\}\}")]
    private static partial Regex ParamsPattern();

    [GeneratedRegex(@"\$\{\{\s*steps\.([a-zA-Z0-9_-]+)\.output\s*\}\}")]
    private static partial Regex StepOutputPattern();

    private static async Task<StepResult> RunStepAsync(
        string command,
        Dictionary<string, string> envVars,
        string workingDir,
        CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "bash",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDir
        };
        psi.ArgumentList.Add("-c");
        psi.ArgumentList.Add(command);

        foreach (var (key, value) in envVars)
        {
            psi.Environment[key] = value;
        }

        using var process = new Process { StartInfo = psi };

        try
        {
            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
            var stderrTask = process.StandardError.ReadToEndAsync(ct);

            await Task.WhenAll(stdoutTask, stderrTask);
            await process.WaitForExitAsync(ct);

            return new StepResult
            {
                Output = (await stdoutTask).TrimEnd(),
                Stderr = (await stderrTask).TrimEnd(),
                ExitCode = process.ExitCode
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new StepResult
            {
                Output = string.Empty,
                Stderr = ex.Message,
                ExitCode = -1
            };
        }
    }

    private sealed class StepResult
    {
        public string Output { get; init; } = string.Empty;
        public string Stderr { get; init; } = string.Empty;
        public int ExitCode { get; init; }
    }
}
