# Navis Navigation Sample

The `Navis.WinUI.Sample` project is a compact, in-memory WinUI 3 walkthrough of the Navis navigation library. It demonstrates a root navigation host containing a shell, a shell-owned child content frame, declarative XAML routes, typed navigation parameters, guarded departures, replacement navigation, and parent/root navigation.

## Navigation graph

```text
Root frame
├── ShellPage
│   └── Child content frame
│       ├── OverviewPage
│       │   └── Parent target ──> StandalonePage
│       ├── ExamplesPage
│       │   └── ExampleDetailsPage (typed NavigationExample)
│       └── DraftPage
└── StandalonePage (root sibling)

StandalonePage ── Root + Reset ──> ShellPage
```

The shell's NavigationView selects pages in its child frame. Overview's Parent target leaves that child frame and opens its root-frame sibling, StandalonePage. Its return action targets Root with Reset, clearing the root journal and restoring the shell. The existing child content frame remains the sample's sole TitleBarNavigation back target; the standalone page does not register a competing root target.

## Capability-to-page map

| Capability | Demonstrated by | What to look for |
| --- | --- | --- |
| Root and child hosts | `MainWindow.xaml`, `ShellPage.xaml` | Nested `NavigationHost` frames and shell/title-bar integration |
| Declarative navigation | `OverviewPage.xaml`, `ShellPage.xaml` | `Navigate.To` attached properties with `XamlType` |
| Typed parameters | `OverviewPage.xaml`, `ExamplesPage.xaml`, `ExampleDetailsPage.xaml` | Bound `Navigate.Parameter` and `INavigationAware<NavigationExample>` |
| Navigation guards | `DraftPage.xaml`, `DraftPageViewModel.cs` | Dirty-state rejection before leaving the draft |
| Replace navigation | `DraftPageViewModel.cs` | Save clears the guard and replaces the draft entry with Overview |
| Parent navigation | `OverviewPage.xaml`, `StandalonePage.xaml` | `Navigate.Target="Parent"` opens the root-level standalone page |
| Root reset | `StandalonePage.xaml` | `Navigate.Target="Root"` and `Navigate.Kind="Reset"` restore `ShellPage` |
| Title-bar integration | `MainWindow.xaml`, sample pages | `TitleBarNavigation` host, pane, back, and content properties |

## Prerequisites

- Windows 10 version 1809 or later with Developer Mode enabled.
- .NET SDK 10.0.
- WinApp CLI 0.6 or later for project-mode launch.

The sample targets `net10.0-windows10.0.19041.0` and supports `x64` and `ARM64`. Restore is handled by the .NET SDK; no additional sample services or network access are required.

## Build

From the repository root:

```powershell
rtk dotnet build Navis.slnx -nologo
rtk dotnet build samples\Navis.WinUI.Sample\Navis.WinUI.Sample.csproj -nologo -p:Platform=x64 -p:RuntimeIdentifier=win-x64
rtk dotnet build samples\Navis.WinUI.Sample\Navis.WinUI.Sample.csproj -nologo -p:Platform=ARM64 -p:RuntimeIdentifier=win-arm64
```

## Run

Build the x64 sample first, then launch its output folder through the installed WinApp CLI. WinApp CLI 0.6 takes an input folder rather than a project file:

```powershell
rtk dotnet build samples\Navis.WinUI.Sample\Navis.WinUI.Sample.csproj -nologo -p:Platform=x64 -p:RuntimeIdentifier=win-x64
```

Register and launch the x64 output with the source manifest:

```powershell
rtk winapp run samples\Navis.WinUI.Sample\bin\x64\Debug\net10.0-windows10.0.19041.0\win-x64 --manifest samples\Navis.WinUI.Sample\Package.appxmanifest --exe Navis.WinUI.Sample.exe --debug-output
```

Use the corresponding ARM64 output folder for ARM64. Do not launch a build-output executable directly.