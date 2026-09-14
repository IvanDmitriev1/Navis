using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public sealed partial class OverviewPageViewModel : ObservableObject, INavigationAware, INavigationAware<string>
{
    private readonly INavigation _navigation;

    public OverviewPageViewModel()
    {
        _navigation = App.Current.GetRequiredService<INavigation>();
    }

    public IReadOnlyList<string> Suggestions { get; } =
    [
        "Declarative routes",
        "Typed parameters",
        "Navigation guards"
    ];

    public NavigationParameterExample FeaturedParameterExample { get; } = NavigationExampleCatalog.Featured;

    [ObservableProperty]
    public partial string? SelectedSuggestion { get; set; }

    [RelayCommand]
    private void SelectSuggestion(AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is not string suggestion)
            return;

        _navigation.Navigate<OverviewPage, string>(suggestion);
    }

    public ValueTask OnNavigatedToAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedToAsync(string parameter, CancellationToken cancellationToken)
    {
        SelectedSuggestion = parameter;
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
