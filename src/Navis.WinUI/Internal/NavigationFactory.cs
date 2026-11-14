namespace Navis.WinUI.Internal;

internal static class NavigationFactory
{
    public static INavigation Create() =>
        // The frame owns its navigator. DI receives a non-disposable proxy.
        new DeferredNavigation(NavigatorActivation.TryGetCurrentFrame(out var currentFrame)
            ? NavigationHost.GetFrameNavigationInstance(currentFrame!)
            : null);
}
