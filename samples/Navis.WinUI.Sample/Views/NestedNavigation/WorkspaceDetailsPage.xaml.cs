using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Navis.WinUI.Sample.Views.NestedNavigation;

public sealed partial class WorkspaceDetailsPage : Page
{
    public WorkspaceDetailsPage()
    {
        ViewModel = App.Current.GetRequiredService<WorkspaceDetailsPageViewModel>();
        InitializeComponent();
        DataContext = ViewModel;
    }

    public WorkspaceDetailsPageViewModel ViewModel { get; }
}
