using System.ComponentModel;

namespace Navis.WinUI.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IParameterizedNavigationAware : INavigationLeavingAware
{
    ValueTask OnNavigatedToAsync(object parameter, CancellationToken cancellationToken);
}

/// <summary>
/// Receives a strongly typed navigation parameter after a page becomes current.
/// </summary>
public interface INavigationAware<TParameter> : IParameterizedNavigationAware
{
    ValueTask OnNavigatedToAsync(TParameter parameter, CancellationToken cancellationToken);

    ValueTask IParameterizedNavigationAware.OnNavigatedToAsync(
        object parameter,
        CancellationToken cancellationToken)
    {
        if (parameter is TParameter typedParameter)
            return OnNavigatedToAsync(typedParameter, cancellationToken);

        throw new InvalidOperationException(
            $"Expected '{typeof(TParameter).FullName}', received '{parameter.GetType().FullName}'.");
    }
}