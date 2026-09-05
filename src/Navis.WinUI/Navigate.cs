namespace Navis.WinUI;

[AttachedDependencyProperty<Type, DependencyObject>("To")]
[AttachedDependencyProperty<object, DependencyObject>("Parameter")]
[AttachedDependencyProperty<NavigationKind, DependencyObject>("Kind", DefaultValue = NavigationKind.Navigate)]
[AttachedDependencyProperty<NavigationTarget, DependencyObject>("Target", DefaultValue = NavigationTarget.Current)]
public static partial class Navigate
{

}
