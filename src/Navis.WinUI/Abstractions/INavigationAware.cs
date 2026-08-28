namespace Navis.WinUI.Abstractions;

/// <summary>
/// Receives synchronous notifications after a page becomes current.
/// </summary>
public interface INavigationAware
{
    ValueTask OnNavigatedToAsync(NavigationContext context);
}

/// <summary>
/// Receives a strongly typed navigation parameter after a page becomes current.
/// </summary>
public interface INavigationAware<TParameter>
{
    ValueTask OnNavigatedToAsync(TParameter parameter, NavigationContext context);
}
