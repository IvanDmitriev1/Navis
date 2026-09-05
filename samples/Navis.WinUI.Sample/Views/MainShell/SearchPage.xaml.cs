using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class SearchPage : Page
{
    public SearchPage()
    {
        InitializeComponent();

        ViewModel = App.Current.GetRequiredService<SearchPageViewModel>();
        DataContext = ViewModel;
    }

    public SearchPageViewModel ViewModel { get; }
}
