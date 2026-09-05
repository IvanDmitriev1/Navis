using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Abstractions;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class LibraryPage : Page
{
    private readonly INavigation _navigation;

    public LibraryPage()
    {
        InitializeComponent();
        _navigation = App.Current.GetRequiredService<INavigation>();
    }

    private void OnBackClick(object sender, RoutedEventArgs e) => _navigation.NavigateBack();
}
