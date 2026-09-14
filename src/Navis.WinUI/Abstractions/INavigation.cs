namespace Navis.WinUI.Abstractions;

/// <summary>
/// Starts navigation operations for one navigation-host Frame.
/// </summary>
/// <remarks>
/// Navigation requests return after validation and are completed asynchronously when guards or
/// navigation-aware callbacks are asynchronous. Requests made while another request is in flight
/// are ignored. Guards run deepest-first before the native navigation commits; aware callbacks run
/// deepest-first on departure and in page order on arrival. Calls must be made on the Frame's UI
/// thread.
/// </remarks>
public interface INavigation
{
    /// <summary>
    /// Gets the top-level navigation host in the current nested host hierarchy.
    /// </summary>
    INavigation Root { get; }

    /// <summary>
    /// Gets the direct parent navigation host, when this host is nested in another enabled frame.
    /// </summary>
    INavigation? Parent { get; }

    /// <summary>
    /// Gets the current live direct child navigation host.
    /// </summary>
    /// <remarks>
    /// The child host detaches when its enabled frame unloads. This property returns <see langword="null" />
    /// after either host is disposed.
    /// </remarks>
    INavigation? Child { get; }

    Type? CurrentPageType { get; }

    bool CanGoBack { get; }

    bool CanGoForward { get; }

    /// <summary>
    /// Navigates to a page type without a parameter.
    /// </summary>
    void Navigate<TPage>(NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page;

    /// <summary>
    /// Navigates to a page type with a strongly typed parameter.
    /// </summary>
    void Navigate<TPage, TParameter>(TParameter parameter, NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page;

    /// <summary>
    /// Navigates to the previous page in the journal.
    /// </summary>
    void NavigateBack();

    /// <summary>
    /// Navigates to the next page in the journal.
    /// </summary>
    void NavigateForward();
}
