namespace Navis.WinUI.Extensions;

internal static class FrameExtensions
{
    public static object? GetNavigationTarget(this Frame frame)
    {
        if (frame.Content is FrameworkElement { DataContext: not null } element)
        {
            return element.DataContext;
        }

        return frame.Content;
    }

    public static async ValueTask<bool> CanNavigate(this Frame frame, CancellationToken token)
    {
        var source = frame.GetNavigationTarget();
        if (source is not INavigationGuard guard)
            return true;

        NavigationDecision decision = await guard.CanNavigateFromAsync(token);
        return decision != NavigationDecision.Reject;
    }
}