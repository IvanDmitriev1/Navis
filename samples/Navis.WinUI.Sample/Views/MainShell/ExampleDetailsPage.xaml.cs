using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class ExampleDetailsPage : Page
{
    public ExampleDetailsPage()
    {
        ViewModel = App.Current.GetRequiredService<ExampleDetailsPageViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public ExampleDetailsPageViewModel ViewModel { get; }
}
