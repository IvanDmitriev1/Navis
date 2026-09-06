namespace Navis.WinUI.Sample.Models;

internal static class NavigationExampleCatalog
{
   public static IReadOnlyList<NavigationExample> All { get; } =
    Array.AsReadOnly(new[]
    {
        new NavigationExample(
            "Declarative Navigation",
            "Use attached properties to navigate from XAML without a click-handler implementation.",
            "Navigate.To"),
        new NavigationExample(
            "Typed Parameters",
            "Pass a strongly typed value to the destination page and receive it through INavigationAware<TParameter>.",
            "INavigation.Navigate<TPage, TParameter>"),
        new NavigationExample(
            "Navigation Guards",
            "Give the current page an opportunity to accept or reject a navigation request before it commits.",
            "INavigationGuard")
    });

    public static NavigationExample Featured => All[1];
}
