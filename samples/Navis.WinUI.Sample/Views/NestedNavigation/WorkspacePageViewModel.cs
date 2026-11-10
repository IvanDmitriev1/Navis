using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.Views.NestedNavigation;

public partial class WorkspacePageViewModel : ObservableObject
{
    private readonly INavigation _navigation = App.Current.GetRequiredService<INavigation>();

    [RelayCommand]
    private void LeaveWorkspace() => _navigation.Navigate<OverviewPage>();
}
