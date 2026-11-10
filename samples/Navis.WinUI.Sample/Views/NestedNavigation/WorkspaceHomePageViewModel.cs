using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;

namespace Navis.WinUI.Sample.Views.NestedNavigation;

public partial class WorkspaceHomePageViewModel : ObservableObject
{
    private readonly INavigation _navigation = App.Current.GetRequiredService<INavigation>();

    [RelayCommand]
    private void OpenDetails() => _navigation.Navigate<WorkspaceDetailsPage, string>("workspace parameter");
}
