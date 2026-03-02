using Gaia.Workflows.Server.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Gaia.Workflows.Server.Parsing;

public static class WorkflowParser
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Parses a YAML workflow definition file.
    /// </summary>
    public static WorkflowDescriptor? Parse(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        try
        {
            var yaml = File.ReadAllText(filePath);
            var descriptor = Deserializer.Deserialize<WorkflowDescriptor>(yaml);

            if (descriptor is null || string.IsNullOrWhiteSpace(descriptor.Description))
                return null;

            if (string.IsNullOrWhiteSpace(descriptor.Name))
                descriptor.Name = Path.GetFileNameWithoutExtension(filePath);

            descriptor.FilePath = filePath;
            return descriptor;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Scans a directory for .yml workflow files and parses each one.
    /// </summary>
    public static List<WorkflowDescriptor> ScanDirectory(string directory)
    {
        if (!Directory.Exists(directory)) return new();

        return Directory.GetFiles(directory, "*.yml")
            .Select(Parse)
            .Where(w => w is not null)
            .Cast<WorkflowDescriptor>()
            .OrderBy(w => w.Name)
            .ToList();
    }
}
