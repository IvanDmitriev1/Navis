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
    /// Performs a journal operation. This overload accepts only
    /// <see cref="NavigationKind.Back"/> and <see cref="NavigationKind.Forward"/>.
    /// </summary>
    void Navigate(NavigationKind kind);

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
