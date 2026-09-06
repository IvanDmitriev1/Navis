using CommunityToolkit.Mvvm.ComponentModel;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

internal partial class ExampleDetailsPageViewModel : ObservableObject, INavigationAware<NavigationExample>
{
    [ObservableProperty]
    public partial NavigationExample? Example { get; set; }

    public ValueTask OnNavigatedToAsync(
        NavigationExample parameter,
        CancellationToken cancellationToken)
    {
        Example = parameter;
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;
}
