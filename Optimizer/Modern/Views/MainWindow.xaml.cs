using System.Windows;
using Optimizer.Modern.ViewModels;

namespace Optimizer.Modern.Views;

public partial class MainWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
