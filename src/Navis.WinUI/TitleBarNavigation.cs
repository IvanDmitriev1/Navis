namespace Navis.WinUI;

[AttachedDependencyProperty<bool, TitleBar>("IsHost")]
[AttachedDependencyProperty<bool, NavigationView>("IsPaneTarget")]
[AttachedDependencyProperty<bool, Frame>("IsBackTarget")]
[AttachedDependencyProperty<UIElement, Page>("Content")]
public static partial class TitleBarNavigation
{
    private sealed class NavigationState
    {
        public TitleBar? Host { get; set; }

        public NavigationView? PaneTarget { get; set; }

        public Frame? BackTarget { get; set; }
    }

    private static readonly DependencyProperty StateProperty =
        DependencyProperty.RegisterAttached(
            "State",
            typeof(NavigationState),
            typeof(TitleBarNavigation),
            new PropertyMetadata(null));

    private static readonly DependencyProperty RegisteredStateProperty =
        DependencyProperty.RegisterAttached(
            "RegisteredState",
            typeof(NavigationState),
            typeof(TitleBarNavigation),
            new PropertyMetadata(null));

    static partial void OnIsHostChanged(TitleBar titleBar, bool newValue) =>
        UpdateRegistration(titleBar, newValue);

    static partial void OnIsPaneTargetChanged(NavigationView navigationView, bool newValue) =>
        UpdateRegistration(navigationView, newValue);

    static partial void OnIsBackTargetChanged(Frame frame, bool newValue) =>
        UpdateRegistration(frame, newValue);

    static partial void OnContentChanged(Page page, UIElement? oldValue, UIElement? newValue)
    {
        if (GetState(page) is { } state && ReferenceEquals(state.BackTarget?.Content, page))
            Refresh(state);
    }

    private static void UpdateRegistration(FrameworkElement element, bool isEnabled)
    {
        element.Loaded -= OnElementLoaded;
        element.Unloaded -= OnElementUnloaded;
        Unregister(element);

        if (!isEnabled)
            return;

        element.Unloaded += OnElementUnloaded;

        if (element.IsLoaded)
            Register(element);
        else
            element.Loaded += OnElementLoaded;
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs args)
    {
        var element = (FrameworkElement)sender;
        element.Loaded -= OnElementLoaded;
        element.Unloaded -= OnElementUnloaded;

        if (!IsEnabled(element))
            return;

        element.Unloaded += OnElementUnloaded;
        Register(element);
    }

    private static void OnElementUnloaded(object sender, RoutedEventArgs args)
    {
        var element = (FrameworkElement)sender;
        element.Loaded -= OnElementLoaded;
        element.Unloaded -= OnElementUnloaded;
        Unregister(element);

        if (IsEnabled(element))
            element.Loaded += OnElementLoaded;
    }

    private static bool IsEnabled(FrameworkElement element) => element switch
    {
        TitleBar titleBar => GetIsHost(titleBar),
        NavigationView navigationView => GetIsPaneTarget(navigationView),
        Frame frame => GetIsBackTarget(frame),
        _ => false
    };

    private static void Register(FrameworkElement element)
    {
        if (element.GetValue(RegisteredStateProperty) is NavigationState)
            return;

        var state = GetOrCreateState(element);

        switch (element)
        {
            case TitleBar titleBar:
                EnsureAvailable(state.Host, titleBar, "title bar host");
                state.Host = titleBar;
                titleBar.BackRequested -= OnBackRequested;
                titleBar.BackRequested += OnBackRequested;
                titleBar.PaneToggleRequested -= OnPaneToggleRequested;
                titleBar.PaneToggleRequested += OnPaneToggleRequested;
                break;

            case NavigationView navigationView:
                EnsureAvailable(state.PaneTarget, navigationView, "pane target");
                state.PaneTarget = navigationView;
                navigationView.DisplayModeChanged -= OnDisplayModeChanged;
                navigationView.DisplayModeChanged += OnDisplayModeChanged;
                navigationView.IsPaneToggleButtonVisible = false;
                break;

            case Frame frame:
                EnsureAvailable(state.BackTarget, frame, "back target");
                state.BackTarget = frame;
                frame.Navigated -= OnFrameNavigated;
                frame.Navigated += OnFrameNavigated;
                break;

            default:
                return;
        }

        element.SetValue(RegisteredStateProperty, state);
        Refresh(state);
    }

    private static void Unregister(FrameworkElement element)
    {
        var state = element.GetValue(RegisteredStateProperty) as NavigationState;

        switch (element)
        {
            case TitleBar titleBar:
                titleBar.BackRequested -= OnBackRequested;
                titleBar.PaneToggleRequested -= OnPaneToggleRequested;

                if (ReferenceEquals(state?.Host, titleBar))
                    state.Host = null;

                titleBar.IsBackButtonVisible = false;
                titleBar.IsBackButtonEnabled = false;
                titleBar.IsPaneToggleButtonVisible = false;
                titleBar.Content = null;
                break;

            case NavigationView navigationView:
                navigationView.DisplayModeChanged -= OnDisplayModeChanged;

                if (ReferenceEquals(state?.PaneTarget, navigationView))
                    state.PaneTarget = null;

                navigationView.IsPaneToggleButtonVisible = true;
                break;

            case Frame frame:
                frame.Navigated -= OnFrameNavigated;

                if (ReferenceEquals(state?.BackTarget, frame))
                    state.BackTarget = null;

                break;
        }

        element.ClearValue(RegisteredStateProperty);

        if (state is not null)
            Refresh(state);
    }

    private static NavigationState GetOrCreateState(FrameworkElement element)
    {
        var rootContent = element.XamlRoot?.Content ??
                          throw new InvalidOperationException(
                              "TitleBarNavigation requires the element to belong to a XamlRoot.");

        if (rootContent.GetValue(StateProperty) is NavigationState state)
            return state;

        state = new NavigationState();
        rootContent.SetValue(StateProperty, state);
        return state;
    }

    private static NavigationState? GetState(FrameworkElement element) =>
        element.XamlRoot?.Content?.GetValue(StateProperty) as NavigationState;

    private static void EnsureAvailable<T>(T? current, T candidate, string role)
        where T : FrameworkElement
    {
        if (current is not null && !ReferenceEquals(current, candidate))
        {
            throw new InvalidOperationException(
                $"A different {role} is already registered for this XamlRoot.");
        }
    }

    private static void OnBackRequested(TitleBar sender, object args)
    {
        if (sender.GetValue(RegisteredStateProperty) is not NavigationState state ||
            state.BackTarget is not { CanGoBack: true } frame)
        {
            return;
        }

        NavigationHost.GetFrameNavigationInstance(frame).NavigateBack();
    }

    private static void OnPaneToggleRequested(TitleBar sender, object args)
    {
        if (sender.GetValue(RegisteredStateProperty) is NavigationState { PaneTarget: { } paneTarget })
            paneTarget.IsPaneOpen = !paneTarget.IsPaneOpen;
    }

    private static void OnDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (sender.GetValue(RegisteredStateProperty) is NavigationState state)
            Refresh(state);
    }

    private static void OnFrameNavigated(object sender, NavigationEventArgs args)
    {
        if (((Frame)sender).GetValue(RegisteredStateProperty) is NavigationState state)
            Refresh(state);
    }

    private static void Refresh(NavigationState state)
    {
        if (state.Host is not { } titleBar)
            return;

        titleBar.IsPaneToggleButtonVisible = state.PaneTarget is { PaneDisplayMode: not NavigationViewPaneDisplayMode.Top };

        var canGoBack = state.BackTarget?.CanGoBack == true;
        titleBar.IsBackButtonVisible = canGoBack;
        titleBar.IsBackButtonEnabled = canGoBack;
        titleBar.Content = state.BackTarget?.Content is Page page ? GetContent(page) : null;
    }
}
