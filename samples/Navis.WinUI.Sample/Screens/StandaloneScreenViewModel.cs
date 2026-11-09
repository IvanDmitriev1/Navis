using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Navis.WinUI.Sample.Messages;

namespace Navis.WinUI.Sample.Screens;

public sealed partial class StandaloneScreenViewModel(IMessenger messenger) : ObservableRecipient(messenger)
{
    [RelayCommand]
    private void ReturnToShell() => Messenger.Send(new ScreenChangeRequestedMessage(SampleScreen.Shell));
}
