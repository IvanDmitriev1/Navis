using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Sample.ViewModels.Authentication;

namespace Navis.WinUI.Sample.Views.Authentication;

public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();

        ViewModel = App.Current.GetRequiredService<LoginViewModel>();
        DataContext = ViewModel;
    }

    public LoginViewModel ViewModel { get; }
}
