using CommunityToolkit.Mvvm.ComponentModel;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public sealed partial class OverviewPageViewModel : ObservableObject, INavigationAware, INavigationAware<string>
{
    public NavigationParameterExample FeaturedParameterExample { get; } = NavigationExampleCatalog.Featured;

    [ObservableProperty]
    public partial string? SelectedSuggestion { get; set; }

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
