namespace Navis.WinUI.Abstractions;

/// <summary>
/// Receives synchronous notifications before the current page is left.
/// </summary>
public interface INavigationLeavingAware
{
    /// <summary>
    /// Runs before the navigation host is mutated.
    /// </summary>
    /// <returns>
    /// Proceed to continue navigation, or Reject to keep the
    /// current navigation state unchanged.
    /// </returns>
    ValueTask<NavigationDecision> OnNavigatingFromAsync(
        NavigationContext context,
        CancellationToken cancellationToken);

    /// <summary>
    /// Runs after navigation has committed.
    /// </summary>
    /// <remarks>
    /// This method cannot reject navigation.
    /// </remarks>
    ValueTask OnNavigatedFromAsync(
        NavigationContext context)
        => ValueTask.CompletedTask;
}
