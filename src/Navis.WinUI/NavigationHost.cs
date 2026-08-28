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

        INavigation? parentNavigation = frame.FindVisualParent<Frame>() is { } parent
            ? GetFrameNavigationInstance(parent)
            : null;

        instance = new FrameNavigation(frame, parentNavigation);
        frame.SetValue(InstanceProperty, instance);

        return instance;
    }

    static partial void OnIsEnabledChanged(Frame frame, bool newValue)
    {
        frame.Loaded -= OnFrameLoaded;

        if (!newValue)
            return;

        if (frame.IsLoaded)
        {
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
        Initialize(frame);
    }

    private static void Initialize(Frame frame)
    {
        var navigation = GetFrameNavigationInstance(frame);
        
        var initialPage = GetInitialPage(frame);
        if (initialPage is null)
            return;

        navigation.Navigate(initialPage);
    }
}
