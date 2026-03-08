namespace Optimizer.Modern.Models;

public sealed class OptimizationTemplate
{
    public required string Name { get; init; }
    public required string SourcePath { get; init; }
    public IReadOnlyList<string> Categories { get; init; } = [];
}
