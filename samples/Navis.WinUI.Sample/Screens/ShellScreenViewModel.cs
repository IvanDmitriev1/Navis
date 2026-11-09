using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Navis.WinUI.Abstractions;
using Navis.WinUI.Sample.Views.MainShell;

namespace Navis.WinUI.Sample.Screens;

public sealed partial class ShellScreenViewModel(INavigation navigation)
{
    public IReadOnlyList<string> Suggestions { get; } = (string[])
    [
        "Declarative routes",
        "Typed parameters",
        "Navigation guards"
    ];

    public void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)

    {
        if (args?.SelectedItem is not string suggestion ||
            string.IsNullOrWhiteSpace(suggestion))
        {
            return;
        }

        navigation.Navigate<OverviewPage, string>(suggestion);
    }
}
