using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

internal partial class DraftPageViewModel : ObservableObject, INavigationGuard
{
    private readonly INavigation _navigation;

    public DraftPageViewModel()
    {
        _navigation = App.Current.GetRequiredService<INavigation>();
    }

    [ObservableProperty]
    public partial string DraftText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsDirty { get; private set; }

    partial void OnDraftTextChanged(string value)
    {
        IsDirty = true;
    }

    public ValueTask<NavigationDecision> CanNavigateFromAsync(CancellationToken cancellationToken) =>
        ValueTask.FromResult(IsDirty ? NavigationDecision.Reject : NavigationDecision.Proceed);

    [RelayCommand]
    private void Save()
    {
        IsDirty = false;
        _navigation.Navigate<OverviewPage>(NavigationKind.Replace);
    }

    [RelayCommand]
    private void Discard()
    {
        IsDirty = false;
        _navigation.NavigateBack();
    }
}
