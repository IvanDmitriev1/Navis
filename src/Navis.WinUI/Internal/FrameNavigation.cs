using Navis.WinUI.Extensions;

namespace Navis.WinUI.Internal;

internal sealed class FrameNavigation : INavigation, IAsyncDisposable
{
    public FrameNavigation(Frame currentFrame, FrameNavigation? parent)
    {
        _currentFrame = currentFrame;
        Attach(parent);
        _hasBeenActivated = false;
    }

    private CancellationTokenSource _cts = new();
    private readonly Frame _currentFrame;
    private FrameNavigation? _parent;
    private FrameNavigation? _child;
    private bool _commitObserved;
    private bool _isCommitting;
    private bool _isNavigating;
    private bool _hasBeenActivated;
    private long _operationId;
    private object? _currentParameter;
    private readonly record struct PendingNavigation(Type PageType, object? Parameter, NavigationKind Kind);

    private PendingNavigation? _pendingNavigation;

    public bool IsDisposed { get; private set; }
    internal bool IsActive { get; private set; }
    internal event EventHandler? NavigationStateChanged;

    public INavigation Root
    {
        get
        {
            FrameNavigation current = this;
            while (current.Parent is not null)
            {
                current = (FrameNavigation)current.Parent;
            }

            return current;
        }
    }

    public INavigation? Parent => _parent is { IsActive: true, IsDisposed: false } parent ? parent : null;
    public INavigation? Child => IsActive && !IsDisposed && _child is { IsActive: true, IsDisposed: false } child ? child : null;

    public Type? CurrentPageType
    {
        get
        {
            EnsureCanOperate();
            return _currentFrame.CurrentSourcePageType;
        }
    }

    public bool CanGoBack
    {
        get
        {
            EnsureCanOperate();
            return FindBackTarget() is not null;
        }
    }

    public bool CanGoForward
    {
        get
        {
            EnsureCanOperate();
            return IsActive && _currentFrame.CanGoForward;
        }
    }

    private bool Attach(FrameNavigation? parent)
    {
        EnsureUiThread();

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(FrameNavigation));

        if (IsActive)
        {
            if (ReferenceEquals(_parent, parent))
            {
                _hasBeenActivated = true;
                return false;
            }

            DetachFromParent();
            AttachToParent(parent);
            RaiseNavigationStateChanged();
            return false;
        }

        AttachToParent(parent);
        if (_hasBeenActivated)
            _cts = new CancellationTokenSource();
        IsActive = true;
        _currentFrame.Navigating += CurrentFrameOnNavigating;
        _currentFrame.Navigated += CurrentFrameOnNavigated;

        bool isRestored = _hasBeenActivated;
        _hasBeenActivated = true;
        RaiseNavigationStateChanged();
        return isRestored;
    }

    internal async ValueTask<bool> ActivateAsync(FrameNavigation? parent, Func<ValueTask> reconnectChildren)
    {
        bool restored = Attach(parent);
        CancellationToken token = _cts.Token;
        var pendingNavigation = !restored ? _pendingNavigation : null;
        if (pendingNavigation is not null)
            _pendingNavigation = null;

        await reconnectChildren();

        bool hadPendingNavigation = pendingNavigation is not null;
        if (pendingNavigation is { } pending && IsActive && !token.IsCancellationRequested)
            await StartNavigation(pending.PageType, pending.Parameter, pending.Kind);

        if (restored && IsActive && !token.IsCancellationRequested && _currentFrame.Content is { } content)
            await content.NotifyNavigatedToAsync(_currentParameter, token);

        return hadPendingNavigation;
    }

    public async ValueTask DisposeAsync()
    {
        EnsureUiThread();

        if (IsDisposed)
            return;

        IsDisposed = true;
        _pendingNavigation = null;
        DetachChildAsRoot();
        await DeactivateAsync();
        DetachFromParent();
        _cts.Dispose();
        RaiseNavigationStateChanged();
    }

    internal async ValueTask DeactivateAsync()
    {
        EnsureUiThread();

        if (!IsActive)
            return;

        IsActive = false;
        _operationId++;
        _isNavigating = false;
        _currentFrame.Navigating -= CurrentFrameOnNavigating;
        _currentFrame.Navigated -= CurrentFrameOnNavigated;
        var cancellation = _cts;
        cancellation.Cancel();
        DetachFromParent();
        RaiseNavigationStateChanged();

        try
        {
            var target = _currentFrame.GetNavigationTarget();
            if (target is INavigationLeavingAware navigationAware)
                await navigationAware.OnNavigatedFromAsync(CancellationToken.None);
        }
        finally
        {
            cancellation.Dispose();
        }
    }

    public void Navigate<TPage>(NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page =>
        _ = StartNavigation(typeof(TPage), null, kind);

    public void Navigate<TPage, TParameter>(TParameter parameter, NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page =>
        _ = StartNavigation(typeof(TPage), parameter, kind);

    public void NavigateBack()
    {
        EnsureCanOperate();
        var target = FindBackTarget();
        if (target is null)
            throw new InvalidOperationException("Cannot navigate back because there is no page in the back stack.");

        _ = target.StartJournalNavigation(NavigationMode.Back);
    }

    public void NavigateForward()
    {
        if (!_currentFrame.CanGoForward)
            throw new InvalidOperationException("Cannot navigate forward because there is no page in the forward stack.");

        _ = StartJournalNavigation(NavigationMode.Forward);
    }

    internal void Navigate(
        Type pageType,
        object? parameter = null,
        NavigationKind kind = NavigationKind.Navigate) =>
        _ = StartNavigation(pageType, parameter, kind);

    private void AttachChild(FrameNavigation child)
    {
        if (_child is { IsActive: true, IsDisposed: false } current && !ReferenceEquals(current, child))
        {
            throw new InvalidOperationException(
                "A navigation host can have only one active child frame.");
        }

        _child = child;
        RaiseNavigationStateChanged();
    }

    private void DetachChild(FrameNavigation child)
    {
        if (ReferenceEquals(_child, child))
        {
            _child = null;
            RaiseNavigationStateChanged();
        }
    }

    private void AttachToParent(FrameNavigation? parent)
    {
        parent?.AttachChild(this);
        _parent = parent;
    }

    private void DetachFromParent()
    {
        var parent = _parent;
        _parent = null;
        parent?.DetachChild(this);
    }

    private void DetachChildAsRoot()
    {
        var child = _child;
        _child = null;

        if (child is null || !ReferenceEquals(child._parent, this))
            return;

        child._parent = null;
        child.RaiseNavigationStateChanged();
        if (child is { IsActive: true, _currentFrame.IsLoaded: true })
            NavigationRootRegistry.Register(child._currentFrame);
    }

    private FrameNavigation? FindBackTarget()
    {
        if (!IsActive || !_currentFrame.IsLoaded)
            return null;

        return (_child is { IsActive: true, IsDisposed: false } child
                   ? child.FindBackTarget()
                   : null) ?? (_currentFrame.CanGoBack ? this : null);
    }

    private void RaiseNavigationStateChanged()
    {
        NavigationStateChanged?.Invoke(this, EventArgs.Empty);
        _parent?.RaiseNavigationStateChanged();
    }

    private async Task StartNavigation(Type pageType, object? parameter, NavigationKind kind)
    {
        EnsureCanOperate();

        if (!_hasBeenActivated)
        {
            _pendingNavigation = new PendingNavigation(pageType, parameter, kind);
            return;
        }

        if (!IsActive)
        {
            return;
        }

        if (_isNavigating)
            return;

        _isNavigating = true;
        long operationId = ++_operationId;
        CancellationToken token = _cts.Token;

        try
        {
            var source = _currentFrame.GetNavigationTarget();

            if (!await CanNavigate(token) || token.IsCancellationRequested || !IsActive)
                return;

            if (!NavigateTo(pageType, parameter, kind))
                return;

            _currentParameter = parameter;
            RaiseNavigationStateChanged();
            var destination = _currentFrame.Content;
            await NotifyNavigationAsync(source, destination, parameter, token);
        }
        finally
        {
            if (_operationId == operationId)
                _isNavigating = false;
        }
    }

    private async Task StartJournalNavigation(NavigationMode mode)
    {
        EnsureCanOperate();

        if (!IsActive || _isNavigating)
            return;

        _isNavigating = true;
        long operationId = ++_operationId;
        CancellationToken token = _cts.Token;

        try
        {
            var source = _currentFrame.GetNavigationTarget();

            if (!await CanNavigate(token) || token.IsCancellationRequested || !IsActive)
                return;

            if (mode == NavigationMode.Back ? !_currentFrame.CanGoBack : !_currentFrame.CanGoForward)
                return;

            var parameter = mode switch
            {
                NavigationMode.Back => _currentFrame.BackStack[^1].Parameter,
                NavigationMode.Forward => _currentFrame.ForwardStack[^1].Parameter,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            _commitObserved = false;
            _isCommitting = true;
            NavigatorActivation.Push(_currentFrame);

            try
            {
                switch (mode)
                {
                    case NavigationMode.Back:
                        _currentFrame.GoBack();
                        break;

                    case NavigationMode.Forward:
                        _currentFrame.GoForward();
                        break;
                }
            }
            finally
            {
                NavigatorActivation.Pop(_currentFrame);
                _isCommitting = false;
            }

            if (!_commitObserved)
                return;

            _currentParameter = parameter;
            RaiseNavigationStateChanged();
            var destination = _currentFrame.Content;
            await NotifyNavigationAsync(source, destination, parameter, token);
        }
        finally
        {
            if (_operationId == operationId)
                _isNavigating = false;
        }
    }

    private async Task<bool> CanNavigate(CancellationToken token)
    {
        if (!IsActive || token.IsCancellationRequested)
            return false;

        if (Child is FrameNavigation child && !await child.CanNavigate(child._cts.Token))
        {
            return false;
        }

        return IsActive && !token.IsCancellationRequested && await _currentFrame.CanNavigate(token);
    }

    private bool NavigateTo(Type pageType, object? parameter, NavigationKind kind)
    {
        bool committed;
        _isCommitting = true;
        NavigatorActivation.Push(_currentFrame);

        try
        {
            if (_currentFrame.CurrentSourcePageType == pageType)
            {
                if (_currentFrame.Content is Page { Content: ScrollViewer scrollViewer })
                {
                    scrollViewer.ChangeView(null, 0, null);
                }

                committed = true;
            }
            else
            {
                committed = _currentFrame.Navigate(pageType, parameter);
            }
        }
        finally
        {
            NavigatorActivation.Pop(_currentFrame);
            _isCommitting = false;
        }

        if (!committed)
            return false;

        switch (kind)
        {
            case NavigationKind.Navigate:
                break;

            case NavigationKind.Replace:
                if (_currentFrame.BackStack.Count > 0)
                {
                    _currentFrame.BackStack.RemoveAt(_currentFrame.BackStack.Count - 1);
                }

                break;

            case NavigationKind.Reset:
                _currentFrame.BackStack.Clear();
                _currentFrame.ForwardStack.Clear();
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(kind), kind, null);
        }

        return true;
    }

    private async Task NotifyNavigationAsync(
        object? source,
        object? destination,
        object? parameter,
        CancellationToken token)
    {
        if (destination is not FrameworkElement { DataContext: { } destinationDataContext })
            return;

        if (!ReferenceEquals(source, destinationDataContext) && source is INavigationLeavingAware sourceAware)
        {
            await sourceAware.OnNavigatedFromAsync(token);
        }

        if (!token.IsCancellationRequested && IsActive && ReferenceEquals(_currentFrame.Content, destination))
            await destination.NotifyNavigatedToAsync(parameter, token);
    }

    private void CurrentFrameOnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        
    }

    private async void CurrentFrameOnNavigated(object sender, NavigationEventArgs e)
    {
        if (_isCommitting)
        {
            _commitObserved = true;
            _currentParameter = e.Parameter;
            return;
        }

        _currentParameter = e.Parameter;
        RaiseNavigationStateChanged();
        CancellationToken token = _cts.Token;
        await e.NotifyNavigatedToAsync(token);
    }

    private void EnsureCanOperate()
    {
        EnsureUiThread();

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(FrameNavigation));
    }

    private void EnsureUiThread()
    {
        if (!_currentFrame.DispatcherQueue.HasThreadAccess)
        {
            throw new InvalidOperationException(
                "Frame navigation must be used from the frame's UI thread.");
        }
    }
}
