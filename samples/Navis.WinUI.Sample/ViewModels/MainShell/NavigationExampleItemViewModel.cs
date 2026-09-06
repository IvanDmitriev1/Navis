using System.Windows.Input;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.ViewModels.MainShell;

public sealed record NavigationExampleItemViewModel(
    NavigationParameterExample ParameterExample,
    ICommand OpenCommand);
