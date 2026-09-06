namespace Navis.WinUI.Abstractions;

/// <summary>
/// Receives synchronous notifications after a page becomes current.
/// </summary>
public interface INavigationAware : INavigationLeavingAware
{
    ValueTask OnNavigatedToAsync(CancellationToken cancellationToken);
}
