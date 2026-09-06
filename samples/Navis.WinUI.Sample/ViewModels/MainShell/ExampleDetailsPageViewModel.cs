using CommunityToolkit.Mvvm.ComponentModel;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public partial class ExampleDetailsPageViewModel : ObservableObject, INavigationAware<NavigationParameterExample>, INavigationAware
{
    [ObservableProperty]
    public partial NavigationParameterExample? Example { get; set; }

    public ValueTask OnNavigatedToAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedToAsync(NavigationParameterExample parameter, CancellationToken cancellationToken)
    {
        Example = parameter;
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;
}
