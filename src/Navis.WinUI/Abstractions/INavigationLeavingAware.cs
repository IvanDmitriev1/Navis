namespace Navis.WinUI.Abstractions;

public interface INavigationLeavingAware
{
    ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken);
}