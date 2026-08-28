using System.Diagnostics.CodeAnalysis;

namespace Navis.WinUI;

[AttachedDependencyProperty<bool, NavigationView>("IsPaneTarget")]
[AttachedDependencyProperty<bool, Frame>("IsBackTarget")]
[AttachedDependencyProperty<UIElement, Page>("Content")]
public static partial class TitleBarNavigation
{
    [field: MaybeNull]
    private static TitleBar TargetTitleBar
    {
        get => field ?? throw new InvalidOperationException("TargetTitleBar is not set");
        set;
    }

    public static void SetTargetTitleBar(TitleBar titleBar)
    {
        
    }
}