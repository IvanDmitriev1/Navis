using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public sealed partial class ShellViewModel(INavigation navigation)
{
    public IReadOnlyList<string> Suggestions { get; } =
    [
        "Declarative routes",
        "Typed parameters",
        "Navigation guards"
    ];

    public void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)

    {
        if (args?.SelectedItem is not string suggestion ||
            string.IsNullOrWhiteSpace(suggestion) ||
            navigation.Child is not { } child)
        {
            return;
        }

        child.Navigate<OverviewPage, string>(suggestion);
    }
}
