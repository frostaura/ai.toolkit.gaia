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
        "Provides the catalogue of Gaia workflows defined in .github/.agaia-workflows/, including each workflow's name, description, parameters, and expected output. " +
        "How to use: call with no arguments; returns a list of WorkflowDescriptor entries you can introspect to build a workflows_run call. " +
        "Use before executing any workflow to discover what is available, what params it expects, and what it produces \u2014 never guess a workflow name.")]
    public List<WorkflowDescriptor> List()
    {
        return WorkflowParser.ScanDirectory(_workflowsDir);
    }

    [McpServerTool(Name = "workflows_run"), Description(
        "Provides end-to-end execution of a named Gaia workflow (a YAML definition under .github/.agaia-workflows/), running each step in order, streaming step status as progress notifications, and returning the captured stdout. " +
        "How to use: pass the workflow name (no .yml extension, discovered via workflows_list) and an optional JSON-object string of args whose keys match the workflow's @param names; args are exposed to steps as environment variables and via ${{ params.<name> }} substitution, and earlier step outputs are available via ${{ steps.<id>.output }}. Returns {ok, output (all step outputs), finalOutput (last step's output)}, or {ok:false, exitCode, failedStep, stdout, stderr} on failure. " +
        "Use to invoke a vetted automation rather than scripting it ad-hoc \u2014 anything from a simple smoke test to a multi-step delivery routine \u2014 and prefer it over inline shell when a workflow already exists. " +
        "Example: workflows_run(name='example-hello', args='{\"name\":\"Dean\",\"greeting\":\"Hi\"}').")]
    public async Task<object> Run(
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
        var finalStepOutput = string.Empty;

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

            // Add previous step outputs as environment variables for safe shell expansion
            var stepEnv = new Dictionary<string, string>(envVars);
            foreach (var (prevStepId, prevOutput) in stepOutputs)
            {
                // Normalize step ID to valid env var name (replace - with _)
                var envName = $"STEP_OUTPUT_{prevStepId.Replace("-", "_")}";
                stepEnv[envName] = prevOutput;
            }

            // Apply ${{ params.<name> }} and ${{ steps.<id>.output }} substitutions
            var command = ApplySubstitutions(step.Run, stepOutputs, envVars);

            // Progress callback for real-time output streaming
            void OnOutputLine(string line)
            {
                progress?.Report(new ProgressNotificationValue
                {
                    Progress = i,
                    Total = totalSteps,
                    Message = $"[{stepId}] {line}"
                });
            }

            var result = await RunStepAsync(command, stepEnv, workingDir, OnOutputLine, cancellationToken);

            stepOutputs[stepId] = result.Output;
            finalStepOutput = result.Output;
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
                    command = command,
                    stdout = result.Output,
                    stderr = result.Stderr,
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
            output = allOutput.ToString().TrimEnd(),
            finalOutput = finalStepOutput
        };
    }

    private static string ApplySubstitutions(string command, Dictionary<string, string> stepOutputs, Dictionary<string, string> envVars)
    {
        // Replace ${{ params.<name> }} with env var reference for safe shell expansion
        // The actual value is already passed via environment variable with the param name
        var result = ParamsPattern().Replace(command, match =>
        {
            var paramName = match.Groups[1].Value;
            if (!envVars.ContainsKey(paramName)) return string.Empty;
            // Return env var reference - bash will safely expand it without re-interpreting
            return $"${paramName}";
        });

        // Replace ${{ steps.<id>.output }} with env var reference for safe shell expansion
        // The actual value is passed via STEP_OUTPUT_<stepId> environment variable
        result = StepOutputPattern().Replace(result, match =>
        {
            var stepId = match.Groups[1].Value;
            if (!stepOutputs.ContainsKey(stepId)) return string.Empty;
            // Return env var reference - bash will safely expand it
            var envName = $"STEP_OUTPUT_{stepId.Replace("-", "_")}";
            return $"${envName}";
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
        Action<string>? onOutputLine,
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

        var stdoutBuilder = new StringBuilder();
        var stderrBuilder = new StringBuilder();

        try
        {
            process.Start();

            // Stream stdout line by line for real-time progress updates
            var stdoutTask = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var line = await process.StandardOutput.ReadLineAsync(ct);
                    if (line is null) break;
                    stdoutBuilder.AppendLine(line);
                    onOutputLine?.Invoke(line);
                }
            }, ct);

            // Capture stderr separately
            var stderrTask = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var line = await process.StandardError.ReadLineAsync(ct);
                    if (line is null) break;
                    stderrBuilder.AppendLine(line);
                    onOutputLine?.Invoke($"[stderr] {line}");
                }
            }, ct);

            await Task.WhenAll(stdoutTask, stderrTask);
            await process.WaitForExitAsync(ct);

            return new StepResult
            {
                Output = stdoutBuilder.ToString().TrimEnd(),
                Stderr = stderrBuilder.ToString().TrimEnd(),
                ExitCode = process.ExitCode
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new StepResult
            {
                Output = stdoutBuilder.ToString().TrimEnd(),
                Stderr = $"{stderrBuilder}\n[exception] {ex.Message}".Trim(),
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
