using Navis.WinUI.Extensions;
using Navis.WinUI.Internal;

namespace Navis.WinUI;

[AttachedDependencyProperty<bool, Frame>("IsEnabled")]
[AttachedDependencyProperty<Type, Frame>("InitialPage")]
public static partial class NavigationHost
{
    private static readonly DependencyProperty InstanceProperty =
        DependencyProperty.RegisterAttached(
            "Instance",
            typeof(FrameNavigation),
            typeof(NavigationHost),
            new PropertyMetadata(null));

    internal static FrameNavigation GetFrameNavigationInstance(Frame frame)
    {
        if (!GetIsEnabled(frame))
            throw new InvalidOperationException("Frame navigation is not enabled.");

        if (frame.GetValue(InstanceProperty) is FrameNavigation instance)
            return instance;

        FrameNavigation? parentNavigation = frame.FindVisualParent<Frame>() is { } parent
            ? GetFrameNavigationInstance(parent)
            : null;

        instance = new FrameNavigation(frame, parentNavigation);
        frame.SetValue(InstanceProperty, instance);

        return instance;
    }

    internal static bool TryGetFrameNavigationInstance(Frame frame, out FrameNavigation instance)
    {
        if (GetIsEnabled(frame) &&
            frame.GetValue(InstanceProperty) is FrameNavigation { IsDisposed: false } navigation)
        {
            instance = navigation;
            return true;
        }

        instance = null!;
        return false;
    }

    static async partial void OnIsEnabledChanged(Frame frame, bool newValue)
    {
        frame.Loaded -= OnFrameLoaded;
        frame.Unloaded -= OnFrameUnloaded;

        if (!newValue)
        {
            await UninitializeAsync(frame);
            return;
        }

        if (frame.IsLoaded)
        {
            frame.Unloaded += OnFrameUnloaded;
            Initialize(frame);
        }
        else
        {
            frame.Loaded += OnFrameLoaded;
        }
    }

    private static void OnFrameLoaded(object sender, RoutedEventArgs e)
    {
        var frame = (Frame)sender;
        frame.Loaded -= OnFrameLoaded;

        if (!GetIsEnabled(frame))
            return;

        frame.Unloaded -= OnFrameUnloaded;
        frame.Unloaded += OnFrameUnloaded;
        Initialize(frame);
    }

    private static async void OnFrameUnloaded(object sender, RoutedEventArgs e)
    {
        var frame = (Frame)sender;
        frame.Loaded -= OnFrameLoaded;
        frame.Unloaded -= OnFrameUnloaded;

        try
        {
            await UninitializeAsync(frame);
        }
        finally
        {
            if (GetIsEnabled(frame))
                frame.Loaded += OnFrameLoaded;
        }
    }

    private static void Initialize(Frame frame)
    {
        var navigation = GetFrameNavigationInstance(frame);

        if (frame.Content is not null)
            return;

        var initialPage = GetInitialPage(frame);
        if (initialPage is null)
            return;

        navigation.Navigate(initialPage);
    }

    private static async ValueTask UninitializeAsync(Frame frame)
    {
        if (frame.GetValue(InstanceProperty) is not FrameNavigation navigation)
            return;

        frame.ClearValue(InstanceProperty);
        await navigation.DisposeAsync();
    }
}
