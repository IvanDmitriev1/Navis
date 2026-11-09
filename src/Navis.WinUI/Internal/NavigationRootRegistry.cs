namespace Navis.WinUI.Internal;

internal static class NavigationRootRegistry
{
    [ThreadStatic]
    private static List<WeakReference<Frame>>? _frames;

    private static List<WeakReference<Frame>> Frames => _frames ??= [];

    public static void Register(Frame frame)
    {
        if (!frame.IsLoaded || !NavigationHost.GetIsEnabled(frame) ||
            !NavigationHost.TryGetFrameNavigationInstance(frame, out var navigation) ||
            navigation.Parent is not null)
            return;

        var frames = Frames;
        RemoveStale(frames);
        if (!frames.Any(reference => reference.TryGetTarget(out var target) && ReferenceEquals(target, frame)))
            frames.Add(new WeakReference<Frame>(frame));
    }

    public static void Unregister(Frame frame)
    {
        var frames = _frames;
        if (frames is null)
            return;

        frames.RemoveAll(reference => !reference.TryGetTarget(out var target) || ReferenceEquals(target, frame));
    }

    public static FrameNavigation Resolve()
    {
        var frames = _frames;
        if (frames is not null)
        {
            RemoveStale(frames);
            var candidates = frames
                .Select(reference => reference.TryGetTarget(out var frame) &&
                                    frame.IsLoaded && NavigationHost.GetIsEnabled(frame) &&
                                    NavigationHost.TryGetFrameNavigationInstance(frame, out var navigation) &&
                                    navigation.Parent is null
                    ? navigation
                    : null)
                .Where(navigation => navigation is not null)
                .Cast<FrameNavigation>()
                .ToArray();

            if (candidates.Length == 1)
                return candidates[0];

            if (candidates.Length > 1)
                throw new InvalidOperationException(
                    "Multiple loaded root navigation frames are available on this UI thread; resolve INavigation during page activation or use a frame-specific navigation service.");
        }

        throw new InvalidOperationException(
            "No loaded, enabled root navigation frame is available on this UI thread.");
    }

    private static void RemoveStale(List<WeakReference<Frame>> frames) =>
        frames.RemoveAll(reference => !reference.TryGetTarget(out var frame) ||
                                      !frame.IsLoaded || !NavigationHost.GetIsEnabled(frame) ||
                                      !NavigationHost.TryGetFrameNavigationInstance(frame, out var navigation) ||
                                      navigation.Parent is not null);
}
