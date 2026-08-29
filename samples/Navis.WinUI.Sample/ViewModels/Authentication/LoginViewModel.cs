using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.Authentication;

public partial class LoginViewModel(INavigation navigation) : ObservableObject, INavigationAware, INavigationGuard
{
    public ValueTask OnNavigatedToAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<NavigationDecision> CanNavigateFromAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(NavigationDecision.Proceed);
    }

    [RelayCommand]
    private void CompleteLogin()
    {
        navigation.Root.Navigate<ShellPage>(NavigationKind.Reset);
    }
}
