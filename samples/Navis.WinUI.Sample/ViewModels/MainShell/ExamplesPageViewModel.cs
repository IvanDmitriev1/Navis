using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

internal partial class ExamplesPageViewModel : ObservableObject
{
    private readonly INavigation _navigation;

    public ExamplesPageViewModel()
    {
        _navigation = App.Current.GetRequiredService<INavigation>();
    }

    public IReadOnlyList<NavigationExample> Examples => NavigationExampleCatalog.All;

    [RelayCommand]
    private void OpenExample(NavigationExample? example)
    {
        if (example is null)
            return;

        _navigation.Navigate<ExampleDetailsPage, NavigationExample>(example);
    }
}
