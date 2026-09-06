using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public sealed class OverviewPageViewModel
{
    public NavigationParameterExample FeaturedParameterExample { get; } = NavigationExampleCatalog.Featured;
}
