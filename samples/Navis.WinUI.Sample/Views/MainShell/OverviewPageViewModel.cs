using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Messages;
using Navis.WinUI.Sample.Models;
using CommunityToolkit.Mvvm.Input;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class OverviewPageViewModel(IMessenger messenger) : ObservableRecipient(messenger), INavigationAware, INavigationAware<string>
{
    public NavigationParameterExample FeaturedParameterExample { get; } = NavigationExampleCatalog.Featured;

    [RelayCommand]
    private void OpenStandalone() => Messenger.Send(new ScreenChangeRequestedMessage(SampleScreen.Standalone));

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
