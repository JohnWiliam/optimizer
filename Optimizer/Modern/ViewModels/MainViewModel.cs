using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Optimizer.Modern.Models;
using Optimizer.Modern.Services;

namespace Optimizer.Modern.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITemplateService _templateService;
    private readonly ICommandRunnerService _commandRunnerService;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _status = "Pronto para Windows 11 25H2+.";

    public ObservableCollection<OptimizationTemplate> Templates { get; } = [];

    public MainViewModel(ITemplateService templateService, ICommandRunnerService commandRunnerService)
    {
        _templateService = templateService;
        _commandRunnerService = commandRunnerService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Templates.Clear();
            var templates = await _templateService.GetAvailableTemplatesAsync();
            foreach (var template in templates)
            {
                Templates.Add(template);
            }

            Status = $"{Templates.Count} template(s) carregado(s).";
        }
        catch (Exception ex)
        {
            Status = $"Falha ao carregar templates: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenTemplatesFolderAsync()
    {
        var result = await _commandRunnerService.OpenTemplateFolderAsync();
        Status = result.Message;
    }
}
