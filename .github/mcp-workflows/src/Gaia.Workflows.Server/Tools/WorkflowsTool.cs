using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Gaia.Workflows.Server.Models;
using Gaia.Workflows.Server.Parsing;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Gaia.Workflows.Server.Tools;

public sealed class WorkflowsTool
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
        "Execute a Gaia workflow by name. The workflow is a bash script in .github/.agaia-workflows/. " +
        "Arguments are passed as a JSON object string where keys match @param names from the workflow " +
        "header, and are set as environment variables for the script. Output is streamed back via " +
        "progress notifications as the script runs, and the final result includes the full output and exit code.")]
    public async Task<object> ExecuteWorkflow(
        [Description("The name of the workflow to execute (without .sh extension). Use workflows_list to discover available names.")] string name,
        [Description("Optional JSON object string of arguments to pass to the workflow. Keys should match the @param names defined in the workflow header. Example: {\"name\": \"Dean\", \"greeting\": \"Hi\"}")] string? args = null,
        IProgress<ProgressNotificationValue>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var scriptPath = Path.Combine(_workflowsDir, $"{name}.sh");
        var descriptor = WorkflowParser.Parse(scriptPath);

        if (descriptor is null)
        {
            return new { ok = false, error = $"Workflow '{name}' not found or has no valid header at {scriptPath}" };
        }

        // Parse args JSON into key-value pairs
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

        var psi = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = scriptPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDir
        };

        foreach (var (key, value) in envVars)
        {
            psi.Environment[key] = value;
        }

        using var process = new Process { StartInfo = psi };

        var allOutput = new StringBuilder();

        try
        {
            process.Start();

            // Read stdout and stderr concurrently, reporting progress for each line
            var stdoutTask = ReadStreamAsync(process.StandardOutput, "stdout", allOutput, progress, cancellationToken);
            var stderrTask = ReadStreamAsync(process.StandardError, "stderr", allOutput, progress, cancellationToken);

            await Task.WhenAll(stdoutTask, stderrTask);
            await process.WaitForExitAsync(cancellationToken);

            return new
            {
                ok = process.ExitCode == 0,
                exitCode = process.ExitCode,
                workflow = name,
                output = allOutput.ToString().TrimEnd()
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new { ok = false, error = $"Failed to execute workflow '{name}': {ex.Message}" };
        }
    }

    private static async Task ReadStreamAsync(
        StreamReader reader,
        string streamName,
        StringBuilder allOutput,
        IProgress<ProgressNotificationValue>? progress,
        CancellationToken ct)
    {
        while (await reader.ReadLineAsync(ct) is { } line)
        {
            var tagged = $"[{streamName}] {line}";
            int currentLine;

            lock (allOutput)
            {
                allOutput.AppendLine(tagged);
                currentLine = allOutput.ToString().Split('\n').Length;
            }

            // Report progress so callers see output in real-time
            progress?.Report(new ProgressNotificationValue
            {
                Progress = currentLine,
                Message = tagged
            });
        }
    }
}
