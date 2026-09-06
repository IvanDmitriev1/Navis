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

    public static ValueTask NotifyNavigatedToAsync(this NavigationEventArgs args, CancellationToken cancellationToken)
    {
        if (args.Content is not FrameworkElement { DataContext: {}  dataContext})
            return ValueTask.CompletedTask;

        if (args.Parameter is null && dataContext is INavigationAware navigationAware)
        {
            return navigationAware.OnNavigatedToAsync(cancellationToken);
        }

        if (args.Parameter is not null && dataContext is IParameterizedNavigationAware navigationAwareWithParameter)
        {
            return navigationAwareWithParameter.OnNavigatedToAsync(args.Parameter, cancellationToken);
        }

        return ValueTask.CompletedTask;
    }
}