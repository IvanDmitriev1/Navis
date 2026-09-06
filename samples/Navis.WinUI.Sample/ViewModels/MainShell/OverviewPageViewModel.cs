using CommunityToolkit.Mvvm.ComponentModel;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public sealed partial class OverviewPageViewModel : ObservableObject, INavigationAware
{
    public NavigationParameterExample FeaturedParameterExample { get; } = NavigationExampleCatalog.Featured;

    public ValueTask OnNavigatedToAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
