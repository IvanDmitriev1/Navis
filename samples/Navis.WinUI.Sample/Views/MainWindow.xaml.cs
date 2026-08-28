using Microsoft.UI.Xaml;

namespace Navis.WinUI.Sample;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);
        TitlebarProperties.SetTargetTitleBar(TitleBar);
    }
}
