namespace Navis.WinUI.Abstractions;

public interface INavigationGuard
{
    ValueTask<NavigationDecision> CanNavigateFromAsync(CancellationToken cancellationToken);
}
