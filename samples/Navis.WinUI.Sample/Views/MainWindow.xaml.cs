using System.Numerics;
using Windows.Foundation;
using Microsoft.UI.Xaml;

namespace Navis.WinUI.Sample;

public sealed partial class MainWindow : Window
{
    private FrameworkElement? _centeredTitleBarContent;
    private float _originalTitleBarContentTranslationX;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);
        TitleBar.LayoutUpdated += OnTitleBarLayoutUpdated;
    }

    private void OnTitleBarLayoutUpdated(object? sender, object args)
    {
        if (TitleBar.Content is not FrameworkElement { HorizontalAlignment: HorizontalAlignment.Center } content)
        {
            RestoreTitleBarContentTranslation();
            return;
        }

        if (!ReferenceEquals(_centeredTitleBarContent, content))
        {
            RestoreTitleBarContentTranslation();
            _centeredTitleBarContent = content;
            _originalTitleBarContentTranslationX = content.Translation.X;
        }

        if (TitleBar.ActualWidth <= 0 || content.ActualWidth <= 0)
            return;

        var contentCenter = content.TransformToVisual(TitleBar)
            .TransformPoint(new Point(content.ActualWidth / 2, 0)).X;
        var deltaX = TitleBar.ActualWidth / 2 - contentCenter;

        if (Math.Abs(deltaX) < 0.5)
            return;

        var translation = content.Translation;
        content.Translation = translation with { X = translation.X + (float)deltaX };
    }

    private void RestoreTitleBarContentTranslation()
    {
        if (_centeredTitleBarContent is not { } content)
            return;

        var translation = content.Translation;
        content.Translation = translation with { X = _originalTitleBarContentTranslationX };
        _centeredTitleBarContent = null;
    }
}
