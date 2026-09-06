namespace Navis.WinUI.Sample.Models;

internal static class NavigationExampleCatalog
{
   public static IReadOnlyList<NavigationParameterExample> All { get; } =
    Array.AsReadOnly([
        new NavigationParameterExample(
            "Declarative Navigation",
            "Use attached properties to navigate from XAML without a click-handler implementation.",
            "Navigate.To"),
        new NavigationParameterExample(
            "Typed Parameters",
            "Pass a strongly typed value to the destination page and receive it through INavigationAware<TParameter>.",
            "INavigation.Navigate<TPage, TParameter>"),
        new NavigationParameterExample(
            "Navigation Guards",
            "Give the current page an opportunity to accept or reject a navigation request before it commits.",
            "INavigationGuard")
    ]);

    public static NavigationParameterExample Featured => All[1];
}
