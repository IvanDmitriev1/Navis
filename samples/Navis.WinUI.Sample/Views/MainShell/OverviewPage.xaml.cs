using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class OverviewPage : Page
{
    public OverviewPage()
    {
        ViewModel = App.Current.GetRequiredService<OverviewPageViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public OverviewPageViewModel ViewModel { get; }
}
