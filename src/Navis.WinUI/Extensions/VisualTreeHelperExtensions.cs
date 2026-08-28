using Navis.WinUI.Internal;

namespace Navis.WinUI.Extensions;

internal static class VisualTreeHelperExtensions
{
    public static T? FindVisualParent<T>(this DependencyObject dp)
        where T : DependencyObject
    {
        var current = VisualTreeHelper.GetParent(dp);
        while (current is not null)
        {
            if (current is T t && !ReferenceEquals(t, dp))
            {
                return t;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    public static FrameNavigation FindFrameNavigation(this DependencyObject dp)
    {
        var current = VisualTreeHelper.GetParent(dp);
        while (current is not null)
        {
            if (current is Page page)
            {
                return NavigationHost.GetFrameNavigationInstance(page.Frame);
            }

            current = VisualTreeHelper.GetParent(current);
        }

        throw new InvalidOperationException(
            "The navigation source is not contained in a Page hosted by a Navis Frame.");
    }
}