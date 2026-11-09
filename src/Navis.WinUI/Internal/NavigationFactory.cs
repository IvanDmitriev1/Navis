namespace Navis.WinUI.Internal;

internal static class NavigationFactory
{
    public static INavigation Create() =>
        NavigatorActivation.TryGetCurrentFrame(out var currentFrame)
            ? NavigationHost.GetFrameNavigationInstance(currentFrame!)
            : new DeferredNavigation();
}
