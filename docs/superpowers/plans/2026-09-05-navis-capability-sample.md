# Navis Focused Capability Sample Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the contrived sample with a compact feature-driven WinUI shell that demonstrates Navis navigation capabilities in context.

**Architecture:** `MainWindow` hosts a root Navis frame whose initial page is `ShellPage`; the shell owns a `NavigationView` and child Navis frame. Overview, Examples, Example Details, and Draft exercise child-frame navigation, while Standalone exercises parent/root navigation. Sample data is deterministic and in memory.

**Tech Stack:** .NET 10, WinUI 3, Windows App SDK 2.4, CommunityToolkit.Mvvm, Microsoft.Extensions.DependencyInjection, Navis.WinUI.

**Spec:** Approved user plan in the parent Codex task; no separate spec file exists.

## Global Constraints

- Preserve project and namespace identity `Navis.WinUI.Sample` and all public `Navis.WinUI` APIs.
- Every implementer and reviewer uses `gpt-5.6-luna` with `xhigh`; no worker dispatches subagents.
- Use only existing dependencies and standard WinUI controls. Add no persistence, network, screenshots, or UI automation.
- Preserve Mica, root/child `NavigationHost` frames, `NavigationViewProperties.Frame`, and `TitleBarNavigation` host/pane/back integration.
- Use clear headings, visible capability callouts, responsive layouts, theme resources, and `AutomationProperties.Name` on interactive controls.
- User explicitly approved build-and-manual-smoke validation instead of automated tests. Do not add tests and do not claim test coverage.
- Prefix every shell command with `rtk`.
- The solution has no `Debug|x64` configuration. Use `rtk dotnet build Navis.slnx -nologo` for task builds and target the sample `.csproj` for architecture-specific builds.
- Do not change or demonstrate forward navigation; `FrameNavigation.CanGoForward` is a separate known defect.

---

### Task 1: Replace the startup and shell structure

**Files:**
- Modify: `samples/Navis.WinUI.Sample/App.xaml.cs`
- Modify: `samples/Navis.WinUI.Sample/Views/MainWindow.xaml`
- Modify: `samples/Navis.WinUI.Sample/Views/MainShell/ShellPage.xaml`
- Create: `samples/Navis.WinUI.Sample/Views/MainShell/OverviewPage.xaml`
- Create: `samples/Navis.WinUI.Sample/Views/MainShell/OverviewPage.xaml.cs`
- Delete: `samples/Navis.WinUI.Sample/Views/Authentication/*`
- Delete: old Home/Search/Library pages and old Authentication/Search ViewModels

**Produces:** A buildable sample that launches `ShellPage`, whose NavigationView contains Overview and whose child frame initially shows `OverviewPage`.

- [ ] Point `MainWindow.RootFrame` directly to `mainShell:ShellPage`; keep it enabled as a Navis host.
- [ ] Remove old Login/Search ViewModel registrations from `App.xaml.cs`.
- [ ] Keep shell title-bar pane/back and NavigationView-to-frame wiring; replace menu content with Overview only for this task.
- [ ] Add a responsive Overview page with `TitleBarNavigation.Content`, a short capability explanation, and a declarative `Navigate.To` placeholder that will target Examples in Task 2.
- [ ] Remove all obsolete Authentication, Home, Search, and Library source files and stale project folder entries.
- [ ] Run `rtk dotnet build Navis.slnx -nologo`; require 0 errors and 0 warnings.
- [ ] Self-review, commit as `feat(sample): replace startup with navigation shell`, and write the task report.

### Task 2: Add declarative and typed parameter examples

**Files:**
- Create: `samples/Navis.WinUI.Sample/Models/NavigationExample.cs`
- Create: `samples/Navis.WinUI.Sample/Models/NavigationExampleCatalog.cs`
- Create: `samples/Navis.WinUI.Sample/ViewModels/MainShell/ExamplesPageViewModel.cs`
- Create: `samples/Navis.WinUI.Sample/ViewModels/MainShell/ExampleDetailsPageViewModel.cs`
- Create: `samples/Navis.WinUI.Sample/Views/MainShell/ExamplesPage.xaml(.cs)`
- Create: `samples/Navis.WinUI.Sample/Views/MainShell/ExampleDetailsPage.xaml(.cs)`
- Modify: shell, Overview, and `App.xaml.cs`

**Interfaces:**
- Produce `internal sealed record NavigationExample(string Title, string Description, string ApiSurface)`.
- Produce catalog entries named `Declarative Navigation`, `Typed Parameters`, and `Navigation Guards`; expose an immutable `IReadOnlyList<NavigationExample>` and the Typed Parameters entry as `Featured`.
- Produce `ExamplesPageViewModel.OpenExampleCommand`, which ignores null and calls `INavigation.Navigate<ExampleDetailsPage, NavigationExample>(example)`.
- Produce `ExampleDetailsPageViewModel : ObservableObject, INavigationAware<NavigationExample>`; arrival sets an observable `Example`, departure returns `ValueTask.CompletedTask`.

- [ ] Add the model and deterministic static catalog using the exact contract above.
- [ ] Add transient Examples and Details ViewModels to DI and resolve them in page constructors using the existing sample pattern.
- [ ] Add Examples to the NavigationView and render catalog entries with an accessible Open command.
- [ ] Complete Overview links: one plain `Navigate.To` route to Examples and one featured Details route using `Navigate.To` plus a bound `Navigate.Parameter`.
- [ ] Bind Details to the typed navigation parameter and display its title, description, API surface, and `TitleBarNavigation.Content`.
- [ ] Run `rtk dotnet build Navis.slnx -nologo`; require 0 errors and 0 warnings.
- [ ] Self-review, commit as `feat(sample): demonstrate typed navigation parameters`, and write the task report.

### Task 3: Add guarded Draft and Replace navigation

**Files:**
- Create: `samples/Navis.WinUI.Sample/ViewModels/MainShell/DraftPageViewModel.cs`
- Create: `samples/Navis.WinUI.Sample/Views/MainShell/DraftPage.xaml`
- Create: `samples/Navis.WinUI.Sample/Views/MainShell/DraftPage.xaml.cs`
- Modify: shell and `App.xaml.cs`

**Interfaces:**
- Produce `DraftPageViewModel : ObservableObject, INavigationGuard` with observable `DraftText` and `IsDirty`.
- `CanNavigateFromAsync` returns `Reject` only while dirty.
- `SaveCommand` clears dirty state, then navigates to Overview with `NavigationKind.Replace`.
- `DiscardCommand` clears dirty state, then calls `NavigateBack`.

- [ ] Register Draft ViewModel as transient and add Draft to the NavigationView.
- [ ] Implement dirty tracking from the first text edit; bind `TextBox.Text` TwoWay with `UpdateSourceTrigger=PropertyChanged`.
- [ ] Show an inline warning whenever the draft is dirty, with visible Save and Discard actions.
- [ ] Ensure Save and Discard clear dirty state before navigation so the guard permits the request.
- [ ] Add `TitleBarNavigation.Content` and accessible names for text and commands.
- [ ] Run `rtk dotnet build Navis.slnx -nologo`; require 0 errors and 0 warnings.
- [ ] Self-review, commit as `feat(sample): demonstrate guarded draft navigation`, and write the task report.

### Task 4: Add Parent/Root navigation, documentation, and final polish

**Files:**
- Create: `samples/Navis.WinUI.Sample/Views/StandalonePage.xaml`
- Create: `samples/Navis.WinUI.Sample/Views/StandalonePage.xaml.cs`
- Modify: Overview, `MainWindow.xaml`, `Package.appxmanifest`, and `README.md`

**Behavior:**
- Overview's standalone button uses `Navigate.Target=Parent` and navigates from the shell child frame to `StandalonePage` in the window root frame.
- Standalone's return button uses `Navigate.Target=Root`, `Navigate.Kind=Reset`, and navigates to `ShellPage`, leaving a clean root journal.

- [ ] Add the standalone flow using attached XAML navigation properties only.
- [ ] Set window title and package display/description copy to `Navis Navigation Sample`; retain technical project/namespace/identity values.
- [ ] Finish responsive spacing, visible capability callouts, theme-safe resources, and accessible control names across all new pages.
- [ ] Expand README with purpose, navigation graph, capability-to-page map, prerequisites, and exact build/run commands.
- [ ] Run `rtk rg -n "Authentication|LoginPage|HomePage|SearchPage|LibraryPage" samples\Navis.WinUI.Sample README.md`; expect no stale references.
- [ ] Run `rtk dotnet build Navis.slnx -nologo`, then architecture builds against the sample `.csproj` for `win-x64` and `win-arm64`; require 0 errors and 0 warnings.
- [ ] Launch through project-mode WinApp tooling, not the executable, and manually verify shell/menu/title-bar, declarative and typed routes, guard/Discard/Replace behavior, Parent standalone transition, and Root Reset restoration.
- [ ] Run `rtk git diff --check`, inspect final status/diff for unrelated changes, self-review, commit as `docs(sample): finish capability walkthrough`, and write the task report.

