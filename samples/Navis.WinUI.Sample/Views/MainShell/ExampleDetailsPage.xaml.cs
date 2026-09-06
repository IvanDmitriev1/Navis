using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class ExampleDetailsPage : Page
{
    public ExampleDetailsPage()
    {
        ViewModel = App.Current.GetRequiredService<ExampleDetailsPageViewModel>();
        InitializeComponent();
    }

    internal ExampleDetailsPageViewModel ViewModel { get; }
}
