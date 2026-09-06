using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class DraftPage : Page
{
    public DraftPage()
    {
        ViewModel = App.Current.GetRequiredService<DraftPageViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    internal DraftPageViewModel ViewModel { get; }
}
