using System.Diagnostics;
using Optimizer.Modern.Models;

namespace Optimizer.Modern.Services;

public sealed class CommandRunnerService : ICommandRunnerService
{
    public Task<ExecutionResult> OpenTemplateFolderAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var templateFolder = Path.Combine(AppContext.BaseDirectory, "Templates");
        Directory.CreateDirectory(templateFolder);

        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            ArgumentList = { templateFolder },
            UseShellExecute = true
        });

        return Task.FromResult(new ExecutionResult
        {
            Success = true,
            Message = "Pasta de templates aberta no Explorer."
        });
    }
}
