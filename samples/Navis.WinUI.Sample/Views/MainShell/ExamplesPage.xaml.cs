using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class ExamplesPage : Page
{
    public ExamplesPage()
    {
        InitializeComponent();

        ViewModel = App.Current.GetRequiredService<ExamplesPageViewModel>();
        DataContext = ViewModel;
    }

    internal ExamplesPageViewModel ViewModel { get; }
}
