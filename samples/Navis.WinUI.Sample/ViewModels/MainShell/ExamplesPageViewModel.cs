using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public partial class ExamplesPageViewModel : ObservableObject
{
    private readonly INavigation _navigation;

    public ExamplesPageViewModel()
    {
        _navigation = App.Current.GetRequiredService<INavigation>();
        Examples = NavigationExampleCatalog.All
            .Select(example => new NavigationExampleItemViewModel(example, OpenExampleCommand))
            .ToArray();
    }

    public IReadOnlyList<NavigationExampleItemViewModel> Examples { get; }

    [RelayCommand]
    private void OpenExample(NavigationParameterExample? example)
    {
        if (example is null)
            return;

        _navigation.Navigate<ExampleDetailsPage, NavigationParameterExample>(example);
    }
}
