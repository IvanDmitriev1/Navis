using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Navis.WinUI.Sample.Views.NestedNavigation;

public sealed partial class WorkspaceHomePage : Page
{
    public WorkspaceHomePage()
    {
        ViewModel = App.Current.GetRequiredService<WorkspaceHomePageViewModel>();
        InitializeComponent();
        DataContext = ViewModel;
    }

    public WorkspaceHomePageViewModel ViewModel { get; }
}
