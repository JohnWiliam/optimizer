using Optimizer.Modern.Models;

namespace Optimizer.Modern.Services;

public interface ITemplateService
{
    Task<IReadOnlyList<OptimizationTemplate>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default);
}
