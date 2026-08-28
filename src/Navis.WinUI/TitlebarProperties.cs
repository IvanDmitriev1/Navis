using System.Diagnostics.CodeAnalysis;

namespace Navis.WinUI;

[AttachedDependencyProperty<Frame>("CurrentNavigationFrame")]
[AttachedDependencyProperty<NavigationView>("CurrentNavigationView")]
public static partial class TitlebarProperties
{
    private static readonly DependencyProperty OwnerElementProperty =
        DependencyProperty.RegisterAttached(
            "OwnerElement",
            typeof(DependencyObject),
            typeof(NavigationViewProperties),
            new PropertyMetadata(null));

    [field: MaybeNull]
    private static TitleBar TargetTitleBar
    {
        get => field ?? throw new InvalidOperationException("TargetTitleBar is not set");
        set;
    }

    public static void SetTargetTitleBar(TitleBar newValue)
    {
        TargetTitleBar = newValue;
        TargetTitleBar.PaneToggleRequested += TitlebarOnPaneToggleRequested;
        TargetTitleBar.BackRequested += TitlebarOnBackRequested;
    }

    static partial void OnCurrentNavigationFrameChanged(DependencyObject dependencyObject, Frame? oldValue, Frame? newValue)
    {
        
    }

    private static void NavigationFrameOnNavigated(object sender, NavigationEventArgs e)
    {
        
    }

    private static void TitlebarOnPaneToggleRequested(TitleBar sender, object args)
    {
        
    }

    private static void TitlebarOnBackRequested(TitleBar sender, object args)
    {

    }
}