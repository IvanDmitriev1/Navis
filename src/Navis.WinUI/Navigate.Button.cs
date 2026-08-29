using Microsoft.UI.Xaml.Controls.Primitives;
using Navis.WinUI.Extensions;
using Navis.WinUI.Internal;

namespace Navis.WinUI;

public partial class Navigate
{
    static partial void OnToChanged(
        DependencyObject dependencyObject,
        Type? oldValue,
        Type? newValue) => UpdateClickHandler(dependencyObject);

    static partial void OnKindChanged(
        DependencyObject dependencyObject,
        NavigationKind oldValue,
        NavigationKind newValue) => UpdateClickHandler(dependencyObject);

    private static void UpdateClickHandler(DependencyObject dependencyObject)
    {
        if (dependencyObject is not ButtonBase button)
            return;

        button.Click -= OnButtonClick;

        var kind = GetKind(button);
        if (GetTo(button) is not null || kind is NavigationKind.Back or NavigationKind.Forward)
        {
            button.Click += OnButtonClick;
        }
    }

    private static void OnButtonClick(object sender, RoutedEventArgs args)
    {
        if (sender is not ButtonBase button)
            return;

        var kind = GetKind(button);
        var destinationPageType = GetTo(button);
        var target = GetTarget(button);
        var parameter = GetParameter(button);

        FrameNavigation navigation = button.FindFrameNavigation();
        navigation = target switch
        {
            NavigationTarget.Current => navigation,
            NavigationTarget.Parent => navigation.Parent as FrameNavigation ??
                                       throw new InvalidOperationException(
                                           "No parent navigation frame is available."),
            NavigationTarget.Root => (FrameNavigation)navigation.Root,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        switch (kind)
        {
            case NavigationKind.Navigate:
            case NavigationKind.Replace:
            case NavigationKind.Reset:
                if (destinationPageType is null)
                {
                    throw new InvalidOperationException(
                        $"Navigate.Kind '{kind}' requires Navigate.To to be set.");
                }

                navigation.Navigate(destinationPageType, parameter, kind);
                break;

            case NavigationKind.Back:
                if (destinationPageType is not null)
                {
                    throw new InvalidOperationException(
                        $"Navigate.Kind '{kind}' does not accept Navigate.To.");
                }

                navigation.NavigateBack();
                break;

            case NavigationKind.Forward:
                if (destinationPageType is not null)
                {
                    throw new InvalidOperationException(
                        $"Navigate.Kind '{kind}' does not accept Navigate.To.");
                }

                navigation.NavigateForward();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }
}
