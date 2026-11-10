using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Navis.WinUI.Sample.Views.NestedNavigation;

public sealed partial class WorkspacePage : Page
{
    public WorkspacePage()
    {
        ViewModel = App.Current.GetRequiredService<WorkspacePageViewModel>();
        InitializeComponent();
        DataContext = ViewModel;
    }

    public WorkspacePageViewModel ViewModel { get; }
}
