using Navis.WinUI.Extensions;
using Navis.WinUI.Internal;

namespace Navis.WinUI;

[AttachedDependencyProperty<bool, Frame>("IsEnabled")]
[AttachedDependencyProperty<Type, Frame>("InitialPage")]
public static partial class NavigationHost
{
    internal static readonly DependencyProperty InstanceProperty =
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

        FrameNavigation? parentNavigation = GetParentNavigation(frame);

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
            await InitializeAsync(frame);
        }
        else
        {
            frame.Loaded += OnFrameLoaded;
        }
    }

    private static async void OnFrameLoaded(object sender, RoutedEventArgs e)
    {
        var frame = (Frame)sender;
        frame.Loaded -= OnFrameLoaded;

        if (!GetIsEnabled(frame))
            return;

        frame.Unloaded -= OnFrameUnloaded;
        frame.Unloaded += OnFrameUnloaded;
        await InitializeAsync(frame);
    }

    private static async void OnFrameUnloaded(object sender, RoutedEventArgs e)
    {
        var frame = (Frame)sender;
        frame.Loaded -= OnFrameLoaded;
        frame.Unloaded -= OnFrameUnloaded;

        try
        {
            NavigationRootRegistry.Unregister(frame);
            if (frame.GetValue(InstanceProperty) is FrameNavigation navigation)
                await navigation.DeactivateAsync();
        }
        finally
        {
            if (GetIsEnabled(frame))
            {
                if (frame.IsLoaded)
                {
                    frame.Unloaded += OnFrameUnloaded;
                    await InitializeAsync(frame);
                }
                else
                {
                    frame.Loaded += OnFrameLoaded;
                }
            }
        }
    }

    private static async ValueTask InitializeAsync(Frame frame)
    {
        var navigation = GetFrameNavigationInstance(frame);
        var parentNavigation = GetParentNavigation(frame);
        await navigation.ActivateAsync(parentNavigation, async () =>
        {
            NavigationRootRegistry.Register(frame);
            await ReattachLoadedChildAsync(frame, navigation);
        });

        if (frame.Content is not null)
            return;

        var initialPage = GetInitialPage(frame);
        if (initialPage is null)
            return;

        navigation.Navigate(initialPage);
    }

    private static async ValueTask UninitializeAsync(Frame frame)
    {
        NavigationRootRegistry.Unregister(frame);

        if (frame.GetValue(InstanceProperty) is not FrameNavigation navigation)
            return;

        frame.ClearValue(InstanceProperty);
        await navigation.DisposeAsync();
    }

    private static FrameNavigation? GetParentNavigation(Frame frame)
    {
        if (frame.FindVisualParent<Frame>() is not { } parent || !GetIsEnabled(parent))
            return null;

        var navigation = GetFrameNavigationInstance(parent);
        return navigation.IsActive ? navigation : null;
    }

    private static async ValueTask ReattachLoadedChildAsync(Frame frame, FrameNavigation navigation)
    {
        if (frame.Content is not DependencyObject content || FindEnabledChildFrame(content) is not { } childFrame)
            return;

        NavigationRootRegistry.Unregister(childFrame);
        var childNavigation = GetFrameNavigationInstance(childFrame);
        await childNavigation.ActivateAsync(navigation, async () =>
        {
            NavigationRootRegistry.Register(childFrame);
            await ReattachLoadedChildAsync(childFrame, childNavigation);
        });
    }

    private static Frame? FindEnabledChildFrame(DependencyObject element)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(element); index++)
        {
            var child = VisualTreeHelper.GetChild(element, index);
            if (child is Frame frame)
            {
                if (GetIsEnabled(frame) && frame.IsLoaded)
                    return frame;

                continue;
            }

            if (FindEnabledChildFrame(child) is { } nestedFrame)
                return nestedFrame;
        }

        return null;
    }
}
