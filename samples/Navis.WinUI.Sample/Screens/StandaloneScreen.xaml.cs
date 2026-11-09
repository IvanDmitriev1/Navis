using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;

namespace Navis.WinUI.Sample.Screens;

public sealed partial class StandaloneScreen : UserControl
{
    public StandaloneScreen()
    {
        ViewModel = new StandaloneScreenViewModel(App.Current.GetRequiredService<IMessenger>());
        InitializeComponent();
    }

    public StandaloneScreenViewModel ViewModel { get; }
}
