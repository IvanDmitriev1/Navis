namespace Navis.WinUI.Abstractions;

public interface INavigationGuard
{
    ValueTask<NavigationDecision> CanNavigateFromAsync(NavigationContext context, CancellationToken cancellationToken);
}