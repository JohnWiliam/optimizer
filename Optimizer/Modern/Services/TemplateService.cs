using System.Text.Json;
using Optimizer.Modern.Models;

namespace Optimizer.Modern.Services;

public sealed class TemplateService : ITemplateService
{
    public async Task<IReadOnlyList<OptimizationTemplate>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templateFolder = Path.Combine(AppContext.BaseDirectory, "Templates");
        if (!Directory.Exists(templateFolder))
        {
            return [];
        }

        var templates = new List<OptimizationTemplate>();
        foreach (var file in Directory.EnumerateFiles(templateFolder, "*.json", SearchOption.TopDirectoryOnly))
        {
            await using var stream = File.OpenRead(file);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var root = json.RootElement;
            var categories = new List<string>();
            if (root.TryGetProperty("settings", out var settings) && settings.ValueKind == JsonValueKind.Object)
            {
                categories.AddRange(settings.EnumerateObject().Select(prop => prop.Name));
            }

            templates.Add(new OptimizationTemplate
            {
                Name = Path.GetFileNameWithoutExtension(file),
                SourcePath = file,
                Categories = categories
            });
        }

        return templates
            .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
