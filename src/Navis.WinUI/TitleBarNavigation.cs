using Navis.WinUI.Internal;

namespace Navis.WinUI;

[AttachedDependencyProperty<bool, TitleBar>("IsHost")]
[AttachedDependencyProperty<bool, NavigationView>("IsPaneTarget")]
[AttachedDependencyProperty<bool, Frame>("IsBackTarget")]
[AttachedDependencyProperty<UIElement, Page>("Content")]
[AttachedDependencyProperty<UIElement, Page>("RightHeader")]
[AttachedDependencyProperty<UIElement, Frame>("PersistentContent")]
[AttachedDependencyProperty<UIElement, Frame>("PersistentRightHeader")]
public static partial class TitleBarNavigation
{
    private sealed class NavigationState
    {
        public TitleBar? Host { get; set; }

        public NavigationView? PaneTarget { get; set; }

        public Frame? BackTarget { get; set; }

        public FrameNavigation? BackNavigation { get; set; }

        public long? BackNavigationInstanceCallbackToken { get; set; }

        public EventHandler? BackNavigationStateChangedHandler { get; set; }
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

    static partial void OnPersistentContentChanged(Frame frame, UIElement? newValue)
    {
        if (GetState(frame) is { } state &&
            ReferenceEquals(state.BackTarget, frame))
        {
            Refresh(state);
        }
    }

    static partial void OnRightHeaderChanged(Page page, UIElement? oldValue, UIElement? newValue)
    {
        if (GetState(page) is { } state && ReferenceEquals(state.BackTarget?.Content, page))
            Refresh(state);
    }

    static partial void OnPersistentRightHeaderChanged(Frame frame, UIElement? newValue)
    {
        if (GetState(frame) is { } state &&
            ReferenceEquals(state.BackTarget, frame))
        {
            Refresh(state);
        }
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
                state.BackNavigationInstanceCallbackToken = frame.RegisterPropertyChangedCallback(
                    NavigationHost.InstanceProperty,
                    OnBackNavigationInstanceChanged);
                AttachNavigation(state, frame.GetValue(NavigationHost.InstanceProperty) as FrameNavigation);
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
                titleBar.RightHeader = null;
                break;

            case NavigationView navigationView:
                navigationView.DisplayModeChanged -= OnDisplayModeChanged;

                if (ReferenceEquals(state?.PaneTarget, navigationView))
                    state.PaneTarget = null;

                navigationView.IsPaneToggleButtonVisible = true;
                break;

            case Frame frame:
                if (state?.BackNavigationInstanceCallbackToken is { } token)
                {
                    frame.UnregisterPropertyChangedCallback(NavigationHost.InstanceProperty, token);
                    state.BackNavigationInstanceCallbackToken = null;
                }

                if (state is not null && ReferenceEquals(state.BackTarget, frame))
                {
                    DetachNavigation(state);
                    state.BackTarget = null;
                }

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
            state.BackNavigation is not { IsActive: true, IsDisposed: false, CanGoBack: true } navigation)
        {
            return;
        }

        navigation.NavigateBack();
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

    private static void OnBackNavigationInstanceChanged(DependencyObject sender, DependencyProperty property)
    {
        if (sender is Frame frame && frame.GetValue(RegisteredStateProperty) is NavigationState state)
            AttachNavigation(state, frame.GetValue(NavigationHost.InstanceProperty) as FrameNavigation);
    }

    private static void OnNavigationStateChanged(NavigationState state, FrameNavigation navigation)
    {
        if (navigation.IsDisposed)
            DetachNavigation(state);

        Refresh(state);
    }

    private static void AttachNavigation(NavigationState state, FrameNavigation? navigation)
    {
        if (ReferenceEquals(state.BackNavigation, navigation))
        {
            Refresh(state);
            return;
        }

        DetachNavigation(state);
        state.BackNavigation = navigation;

        if (navigation is not null)
        {
            EventHandler handler = (_, _) => OnNavigationStateChanged(state, navigation);
            state.BackNavigationStateChangedHandler = handler;
            navigation.NavigationStateChanged += handler;
        }

        Refresh(state);
    }

    private static void DetachNavigation(NavigationState state)
    {
        if (state.BackNavigation is { } navigation && state.BackNavigationStateChangedHandler is { } handler)
            navigation.NavigationStateChanged -= handler;

        state.BackNavigation = null;
        state.BackNavigationStateChangedHandler = null;
    }

    private static void Refresh(NavigationState state)
    {
        if (state.Host is not { } titleBar)
            return;

        titleBar.IsPaneToggleButtonVisible = state.PaneTarget is { PaneDisplayMode: not NavigationViewPaneDisplayMode.Top };

        var canGoBack = state.BackNavigation is { IsActive: true, IsDisposed: false, CanGoBack: true };
        titleBar.IsBackButtonVisible = canGoBack;
        titleBar.IsBackButtonEnabled = canGoBack;

        if (state.BackTarget is not { } frame)
        {
            titleBar.Content = null;
            titleBar.RightHeader = null;
            return;
        }

        var page = frame.Content as Page;
        var pageContent = page is null ? null : GetContent(page);
        var pageRightHeader = page is null ? null : GetRightHeader(page);

        titleBar.Content = pageContent ?? GetPersistentContent(frame);
        titleBar.RightHeader = pageRightHeader ?? GetPersistentRightHeader(frame);

        if (titleBar.Content is FrameworkElement contentElement)
        {
            contentElement.DataContext = pageContent is not null
                ? page?.DataContext
                : frame.DataContext;
        }

        if (titleBar.RightHeader is FrameworkElement rightHeaderElement)
        {
            rightHeaderElement.DataContext = pageRightHeader is not null
                ? page?.DataContext
                : frame.DataContext;
        }
    }
}
