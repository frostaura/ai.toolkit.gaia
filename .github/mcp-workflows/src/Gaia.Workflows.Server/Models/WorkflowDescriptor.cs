using YamlDotNet.Serialization;

namespace Gaia.Workflows.Server.Models;

public sealed class WorkflowDescriptor
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "description")]
    public string Description { get; set; } = string.Empty;

    [YamlIgnore]
    public string FilePath { get; set; } = string.Empty;

    [YamlMember(Alias = "params")]
    public List<WorkflowParam> Params { get; set; } = new();

    [YamlMember(Alias = "output")]
    public string? Output { get; set; }

    [YamlMember(Alias = "steps")]
    public List<StepDescriptor> Steps { get; set; } = new();
}

public sealed class WorkflowParam
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class StepDescriptor
{
    [YamlMember(Alias = "id")]
    public string Id { get; set; } = string.Empty;

    [YamlMember(Alias = "run")]
    public string Run { get; set; } = string.Empty;
}
