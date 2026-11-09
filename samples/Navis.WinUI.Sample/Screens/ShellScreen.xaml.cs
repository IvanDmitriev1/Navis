using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Abstractions;

namespace Navis.WinUI.Sample.Screens;

public sealed partial class ShellScreen : UserControl
{
    public ShellScreen()
    {
        ViewModel = new ShellScreenViewModel(App.Current.GetRequiredService<INavigation>());
        InitializeComponent();
    }

    public ShellScreenViewModel ViewModel { get; }
}
