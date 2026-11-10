namespace Navis.WinUI.Abstractions;

/// <summary>
/// Starts navigation operations for one navigation-host Frame.
/// </summary>
/// <remarks>
/// Navigation requests target this host's frame, except back navigation, which searches this host
/// and its loaded child hosts deepest-first. Requests return after validation and are completed
/// asynchronously when guards or navigation-aware callbacks are asynchronous. Requests made while
/// another request is in flight are ignored. Guards run deepest-first before the native navigation
/// commits; aware callbacks run deepest-first on departure and in page order on arrival. Calls must
/// be made on the Frame's UI thread.
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
    /// The child host detaches while its frame is unloaded and reconnects when it loads again. This
    /// property returns <see langword="null" /> when there is no active child or either host is disposed.
    /// </remarks>
    INavigation? Child { get; }

    /// <summary>
    /// Gets the current page type in this host's frame.
    /// </summary>
    Type? CurrentPageType { get; }

    /// <summary>
    /// Gets whether this host or a loaded child host has a page in its back journal.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets whether this host's frame has a page in its forward journal.
    /// </summary>
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
    /// Navigates back in the deepest loaded child frame with back history, or in this frame when
    /// no child frame can go back.
    /// </summary>
    void NavigateBack();

    /// <summary>
    /// Navigates to the next page in this frame's forward journal.
    /// </summary>
    void NavigateForward();
}
