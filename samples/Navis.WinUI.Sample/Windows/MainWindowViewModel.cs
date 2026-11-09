using Navis.WinUI.Sample.Screens;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.Messages;

namespace Navis.WinUI.Sample.Windows;

public sealed partial class MainWindowViewModel : ObservableRecipient, IRecipient<ScreenChangeRequestedMessage>
{
    public MainWindowViewModel(IMessenger messenger) : base(messenger) => IsActive = true;

    [ObservableProperty]
    public partial UserControl? CurrentScreen { get; set; }

    [RelayCommand]
    private void ShowShell() => CurrentScreen = new ShellScreen();

    [RelayCommand]
    private void ShowStandalone() => CurrentScreen = new StandaloneScreen();

    public void Receive(ScreenChangeRequestedMessage message)
    {
        switch (message.Value)
        {
            case SampleScreen.Shell:
                ShowShell();
                break;
            case SampleScreen.Standalone:
                ShowStandalone();
                break;
        }
    }
}
