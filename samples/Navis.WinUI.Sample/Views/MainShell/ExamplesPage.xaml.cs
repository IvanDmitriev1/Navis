using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.Models;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class ExamplesPage : Page
{
    public ExamplesPage()
    {
        ViewModel = App.Current.GetRequiredService<ExamplesPageViewModel>();
        InitializeComponent();
    }

    internal ExamplesPageViewModel ViewModel { get; }

    private void OpenExampleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
    {
        if (sender is Button { DataContext: NavigationExample example })
            ViewModel.OpenExampleCommand.Execute(example);
    }
}
