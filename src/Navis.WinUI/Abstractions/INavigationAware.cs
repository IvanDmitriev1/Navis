namespace Navis.WinUI.Abstractions;

/// <summary>
/// Receives synchronous notifications after a page becomes current.
/// </summary>
public interface INavigationAware
{
    ValueTask OnNavigatedToAsync(NavigationContext context, CancellationToken cancellationToken);

    ValueTask OnNavigatedFromAsync(NavigationContext context, CancellationToken cancellationToken);
}

/// <summary>
/// Receives a strongly typed navigation parameter after a page becomes current.
/// </summary>
public interface INavigationAware<TParameter> : INavigationAware
{
    ValueTask OnNavigatedToAsync(TParameter parameter, NavigationContext context, CancellationToken cancellationToken);

    ValueTask INavigationAware.OnNavigatedToAsync(
        NavigationContext context,
        CancellationToken cancellationToken) =>
        OnNavigatedToAsync(context.GetParameter<TParameter>(), context, cancellationToken);
}
