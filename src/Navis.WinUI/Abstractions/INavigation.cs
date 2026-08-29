namespace Navis.WinUI.Abstractions;

/// <summary>
/// Starts navigation operations for one navigation-host Frame.
/// </summary>
public interface INavigation
{
    INavigation Root { get; }

    INavigation? Parent { get; }

    Type? CurrentPageType { get; }

    bool CanGoBack { get; }

    bool CanGoForward { get; }

    /// <summary>
    /// Navigates to the previous page in the journal.
    /// </summary>
    void NavigateBack();

    /// <summary>
    /// Navigates to the next page in the journal.
    /// </summary>
    void NavigateForward();

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
}
