namespace Navis.WinUI.Internal;

internal sealed class DeferredNavigation(FrameNavigation? navigation) : INavigation
{
    private FrameNavigation Resolve() => navigation ?? NavigationRootRegistry.Resolve();

    public INavigation Root => Resolve().Root;
    public INavigation? Parent => Resolve().Parent;
    public INavigation? Child => Resolve().Child;
    public Type? CurrentPageType => Resolve().CurrentPageType;
    public bool CanGoBack => Resolve().CanGoBack;
    public bool CanGoForward => Resolve().CanGoForward;

    public void Navigate<TPage>(NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page => Resolve().Navigate<TPage>(kind);

    public void Navigate<TPage, TParameter>(TParameter parameter, NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page => Resolve().Navigate<TPage, TParameter>(parameter, kind);

    public void NavigateBack() => Resolve().NavigateBack();
    public void NavigateForward() => Resolve().NavigateForward();
}
