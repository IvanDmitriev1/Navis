namespace Navis.WinUI.Abstractions;

/// <summary>
/// Receives synchronous notifications after a page becomes current.
/// </summary>
public interface INavigationAware
{
    ValueTask OnNavigatedToAsync(object? parameter, CancellationToken cancellationToken);

    ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Receives a strongly typed navigation parameter after a page becomes current.
/// </summary>
public interface INavigationAware<TParameter> : INavigationAware
{
    ValueTask OnNavigatedToAsync(TParameter parameter, CancellationToken cancellationToken);

    ValueTask INavigationAware.OnNavigatedToAsync(
        object? parameter,
        CancellationToken cancellationToken) =>
        OnNavigatedToAsync(GetParameter(parameter), cancellationToken);

    private static TParameter GetParameter(object? parameter)
    {
        if (parameter is TParameter typedParameter)
            return typedParameter;

        throw new InvalidOperationException(
            $"The navigation parameter value cannot be read as " +
            $"'{typeof(TParameter).FullName}'.");
    }
}
