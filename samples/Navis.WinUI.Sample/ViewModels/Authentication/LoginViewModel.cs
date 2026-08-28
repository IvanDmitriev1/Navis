using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.Authentication;

public partial class LoginViewModel(INavigation navigation) : ObservableObject, INavigationAware, INavigationLeavingAware
{
    public ValueTask OnNavigatedToAsync(NavigationContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<NavigationDecision> OnNavigatingFromAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void CompleteLogin()
    {
        navigation.Root.Navigate<ShellPage>(NavigationKind.Reset);
    }
}
