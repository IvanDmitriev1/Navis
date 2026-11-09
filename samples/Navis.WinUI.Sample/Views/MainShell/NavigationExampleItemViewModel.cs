using System.Windows.Input;
using Navis.WinUI.Sample.Models;

namespace Navis.WinUI.Sample.Views.MainShell;

public sealed record NavigationExampleItemViewModel(
    NavigationParameterExample ParameterExample,
    ICommand OpenCommand);
