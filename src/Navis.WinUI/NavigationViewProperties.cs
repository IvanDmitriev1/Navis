namespace Navis.WinUI;

[AttachedDependencyProperty<Frame, NavigationView>("Frame")]
[AttachedDependencyProperty<Type, Page>("OwnerPage")]
public static partial class NavigationViewProperties
{
    private static readonly DependencyProperty NavigationViewProperty =
        DependencyProperty.RegisterAttached(
            "NavigationView",
            typeof(NavigationView),
            typeof(NavigationViewProperties),
            new PropertyMetadata(null));

    static partial void OnFrameChanged(NavigationView navigationView, Frame? oldValue, Frame? newValue)
    {
        navigationView.ItemInvoked -= NavigationView_ItemInvoked;

        if (oldValue is not null)
        {
            oldValue.Navigated -= ContentFrame_OnNavigated;
            oldValue.ClearValue(NavigationViewProperty);
        }

        if (newValue is null)
            return;

        navigationView.ItemInvoked += NavigationView_ItemInvoked;
        newValue.Navigated += ContentFrame_OnNavigated;
        newValue.SetValue(NavigationViewProperty, navigationView);
    }

    private static void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (GetFrame(sender) is not { } frame ||
            args.IsSettingsInvoked ||
            args.InvokedItemContainer is not DependencyObject item)
        {
            return;
        }

        var destinationPageType = Navigate.GetTo(item) ??
                                  throw new InvalidOperationException("Failed to get the destination page type.");
        var parameter = Navigate.GetParameter(item);

        var navigationInstance = NavigationHost.GetFrameNavigationInstance(frame);
        if (destinationPageType == navigationInstance.CurrentPageType)
            return;

        navigationInstance.Navigate(destinationPageType, parameter);
    }

    private static void ContentFrame_OnNavigated(object sender, NavigationEventArgs args)
    {
        var frame = (Frame)sender;
        if (frame.GetValue(NavigationViewProperty) is not NavigationView navigationView)
            return;

        var pageType = args.Content is Page page
            ? GetOwnerPage(page) ?? args.SourcePageType
            : args.SourcePageType;
        var selectedItem = FindNavigationViewItem(navigationView.MenuItems, pageType) ??
                           FindNavigationViewItem(navigationView.FooterMenuItems, pageType);

        if (selectedItem is not null)
        {
            navigationView.SelectedItem = selectedItem;
        }
    }

    private static NavigationViewItem? FindNavigationViewItem(IEnumerable<object> items, Type pageType)
    {
        foreach (var item in items)
        {
            if (item is not NavigationViewItem navigationViewItem)
                continue;

            if (Navigate.GetTo(navigationViewItem) == pageType)
                return navigationViewItem;

            if (FindNavigationViewItem(navigationViewItem.MenuItems, pageType) is { } nestedItem)
                return nestedItem;
        }

        return null;
    }
}
