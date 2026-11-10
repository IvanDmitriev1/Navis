using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;

namespace Navis.WinUI.Sample.Views.NestedNavigation;

public partial class WorkspaceDetailsPageViewModel : ObservableObject, INavigationAware<string>
{
    private readonly INavigation _navigation = App.Current.GetRequiredService<INavigation>();

    [ObservableProperty]
    public partial string? ReceivedParameter { get; set; }

    public ValueTask OnNavigatedToAsync(string parameter, CancellationToken cancellationToken)
    {
        ReceivedParameter = parameter;
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    [RelayCommand]
    private void OpenHome() => _navigation.Navigate<WorkspaceHomePage>();
}
