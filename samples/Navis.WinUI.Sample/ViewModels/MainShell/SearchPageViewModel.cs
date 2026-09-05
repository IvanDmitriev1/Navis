using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public partial class SearchPageViewModel : ObservableObject, INavigationAware, INavigationGuard
{
    private INavigation Navigation { get; } = App.Current.GetRequiredService<INavigation>();


    public ValueTask OnNavigatedToAsync(object? parameter, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<NavigationDecision> CanNavigateFromAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(NavigationDecision.Reject);
    }

    [RelayCommand]
    private void Back()
    {
        Navigation.NavigateBack();
    }
}
