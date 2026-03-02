using Gaia.Workflows.Server.Models;

namespace Gaia.Workflows.Server.Parsing;

public static class WorkflowParser
{
    /// <summary>
    /// Parses the comment header of a bash script to extract workflow metadata.
    /// Expected format:
    ///   #!/bin/bash
    ///   # @name: workflow-name
    ///   # @description: What this workflow does
    ///   # @param arg_name: Description of the argument
    ///   # @output: What the workflow outputs
    /// </summary>
    public static WorkflowDescriptor? Parse(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        var lines = File.ReadAllLines(filePath);
        string? name = null;
        string? description = null;
        string? output = null;
        var parameters = new List<WorkflowParam>();

        foreach (var line in lines)
        {
            var stripped = line.Trim();

            // Skip shebang
            if (stripped.StartsWith("#!")) continue;

            // Stop at the first non-comment, non-empty line
            if (!stripped.StartsWith("#") && stripped.Length > 0) break;

            // Skip empty lines and bare comments
            if (stripped is "#" or "") continue;

            var content = stripped.TrimStart('#').Trim();

            if (content.StartsWith("@name:", StringComparison.OrdinalIgnoreCase))
            {
                name = content["@name:".Length..].Trim();
            }
            else if (content.StartsWith("@description:", StringComparison.OrdinalIgnoreCase))
            {
                description = content["@description:".Length..].Trim();
            }
            else if (content.StartsWith("@param ", StringComparison.OrdinalIgnoreCase))
            {
                var paramPart = content["@param ".Length..].Trim();
                var colonIndex = paramPart.IndexOf(':');
                if (colonIndex > 0)
                {
                    parameters.Add(new WorkflowParam
                    {
                        Name = paramPart[..colonIndex].Trim(),
                        Description = paramPart[(colonIndex + 1)..].Trim()
                    });
                }
            }
            else if (content.StartsWith("@output:", StringComparison.OrdinalIgnoreCase))
            {
                output = content["@output:".Length..].Trim();
            }
        }

        // Default name from filename if not specified
        name ??= Path.GetFileNameWithoutExtension(filePath);

        // Require at least a description
        if (description is null) return null;

        return new WorkflowDescriptor
        {
            Name = name,
            Description = description,
            FilePath = filePath,
            Params = parameters,
            Output = output
        };
    }

    /// <summary>
    /// Scans a directory for .sh files and parses each one.
    /// </summary>
    public static List<WorkflowDescriptor> ScanDirectory(string directory)
    {
        if (!Directory.Exists(directory)) return new();

        return Directory.GetFiles(directory, "*.sh")
            .Select(Parse)
            .Where(w => w is not null)
            .Cast<WorkflowDescriptor>()
            .OrderBy(w => w.Name)
            .ToList();
    }
}
