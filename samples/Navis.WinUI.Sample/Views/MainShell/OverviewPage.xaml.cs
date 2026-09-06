using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed partial class OverviewPage : Page
{
    internal NavigationExample FeaturedExample => NavigationExampleCatalog.Featured;

    public OverviewPage()
    {
        InitializeComponent();
    }
}
