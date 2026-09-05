using Microsoft.UI.Xaml.Controls.Primitives;
using Navis.WinUI.Extensions;
using Navis.WinUI.Internal;

namespace Navis.WinUI;

public partial class Navigate
{
    static partial void OnToChanged(DependencyObject dependencyObject) =>
        UpdateClickHandler(dependencyObject);

    static partial void OnKindChanged(DependencyObject dependencyObject) =>
        UpdateClickHandler(dependencyObject);

    private static void UpdateClickHandler(DependencyObject dependencyObject)
    {
        if (dependencyObject is not ButtonBase button)
            return;

        button.Click -= OnButtonClick;

        if (GetTo(button) is not null)
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

            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }
}
