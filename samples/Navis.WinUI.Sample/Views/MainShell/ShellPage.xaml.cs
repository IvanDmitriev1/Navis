using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        ViewModel = App.Current.GetRequiredService<ShellViewModel>();
        InitializeComponent();
    }

    public ShellViewModel ViewModel { get; }
}
