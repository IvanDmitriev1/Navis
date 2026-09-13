# Navis.WinUI

Install the NuGet package:

```powershell
dotnet add package Navis.WinUI
```

Navis.WinUI is a compact WinUI 3 navigation library for typed, nested, and declarative page navigation.

## Quick usage

Register navigation with the application's service collection. Injected `INavigation` is contextual: it resolves the navigation host for the currently active frame.

```csharp
using Navis.WinUI.Extensions;

services.AddNavisNavigation();
```

Enable navigation on a `Frame` and optionally provide its first page. `XamlType` converts a XAML type name to the `Type` expected by Navis:

```xml
<Page
    xmlns:main="using:YourApp.Views"
    xmlns:nav="using:Navis.WinUI">
    <Frame
        nav:NavigationHost.IsEnabled="True"
        nav:NavigationHost.InitialPage="{nav:XamlType TypeName=main:HomePage}" />
</Page>
```

Use the contextual navigation service from a page or view model:

```csharp
public sealed class HomeViewModel(INavigation navigation)
{
    public void OpenDetails() => navigation.Navigate<DetailsPage>();

    public void OpenOrder(Order order) =>
        navigation.Navigate<DetailsPage, Order>(order);
}
```

`Navigate<TPage>()` navigates without a parameter. `Navigate<TPage, TParameter>(parameter)` passes a strongly typed value to the destination. Both methods also accept a `NavigationKind` such as `Replace` or `Reset`.

Receive a typed parameter and participate in the navigation lifecycle with `INavigationAware<TParameter>`:

```csharp
public sealed class DetailsViewModel : INavigationAware<Order>
{
    public ValueTask OnNavigatedToAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // Load the order.
        return ValueTask.CompletedTask;
    }

    public ValueTask OnNavigatedFromAsync(CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;
}
```

## XAML navigation

Attach navigation to a button or other button-based control. `Navigate.To` selects the page, `Navigate.Kind` selects the journal operation, and `Navigate.Target` selects the current, parent, or root navigation host:

```xml
<Button
    xmlns:pages="using:YourApp.Views"
    xmlns:nav="using:Navis.WinUI"
    Content="Open details"
    nav:Navigate.To="{nav:XamlType TypeName=pages:DetailsPage}"
    nav:Navigate.Kind="Navigate"
    nav:Navigate.Target="Current" />
```

Parameters can be bound in XAML with `Navigate.Parameter`:

```xml
<Button
    nav:Navigate.Parameter="{x:Bind ViewModel.SelectedOrder}"
    nav:Navigate.To="{nav:XamlType TypeName=pages:DetailsPage}" />
```

## Caveats

- `INavigation` is contextual. Resolve or inject it where the intended frame is active; it is not a global navigation service.
- Navigation must run on the frame's UI thread.
- Requests made while another navigation request is in flight are ignored.
- Typed parameters must match the destination's `INavigationAware<TParameter>` type at runtime.
- `Navigate.Target="Parent"` requires a nested navigation host with a parent frame. `Root` resolves the top-level host, so parent/root routing requires the corresponding nested-frame structure.
