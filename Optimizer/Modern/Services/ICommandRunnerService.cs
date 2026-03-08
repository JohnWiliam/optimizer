using Optimizer.Modern.Models;

namespace Optimizer.Modern.Services;

public interface ICommandRunnerService
{
    Task<ExecutionResult> OpenTemplateFolderAsync(CancellationToken cancellationToken = default);
}
