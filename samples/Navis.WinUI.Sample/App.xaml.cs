using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Navis.WinUI.Extensions;
using Navis.WinUI.Sample.ViewModels.MainShell;

namespace Navis.WinUI.Sample;

public partial class App : Application, IServiceProvider
{
    public new static App Current => (App)Microsoft.UI.Xaml.Application.Current;

    private readonly IServiceProvider _serviceProvider;
    private readonly AppActivationArguments _initialActivation;
    private Window? _window;

    public App(AppActivationArguments initialActivation)
    {
        UnhandledException += OnUnhandledException;

        InitializeComponent();
        _initialActivation = initialActivation;

        _serviceProvider = new ServiceCollection()
            .AddNavisNavigation()
            .AddTransient<ShellViewModel>()
            .AddTransient<OverviewPageViewModel>()
            .AddTransient<ExamplesPageViewModel>()
            .AddTransient<ExampleDetailsPageViewModel>()
            .AddTransient<DraftPageViewModel>()
            .BuildServiceProvider();
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        HandleActivation(_initialActivation);
    }

    public void HandleActivation(AppActivationArguments args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    private static void OnUnhandledException(
        object sender,
        Microsoft.UI.Xaml.UnhandledExceptionEventArgs args) =>
        Debug.WriteLine($"Navis sample unhandled exception: {args.Exception}");
}
