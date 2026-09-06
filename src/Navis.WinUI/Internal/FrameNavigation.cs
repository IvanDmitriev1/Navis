using Navis.WinUI.Extensions;

namespace Navis.WinUI.Internal;

internal sealed class FrameNavigation : INavigation, IAsyncDisposable
{
    public FrameNavigation(Frame currentFrame, FrameNavigation? parent)
    {
        _currentFrame = currentFrame;
        _parent = parent;

        _parent?.AttachChild(this);

        _currentFrame.Navigating += CurrentFrameOnNavigating;
        _currentFrame.Navigated += CurrentFrameOnNavigated;
    }

    private readonly CancellationTokenSource _cts = new();
    private readonly Frame _currentFrame;
    private readonly FrameNavigation? _parent;
    private FrameNavigation? _child;

    public bool IsDisposed { get; private set; }

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

    public INavigation? Parent => _parent;

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
            return _currentFrame.CanGoBack;
        }
    }

    public bool CanGoForward
    {
        get
        {
            EnsureCanOperate();
            return _currentFrame.CanGoBack;
        }
    }

    public async ValueTask DisposeAsync()
    {
        EnsureUiThread();

        if (IsDisposed)
            return;

        IsDisposed = true;
        _parent?.DetachChild(this);

        _currentFrame.Navigating -= CurrentFrameOnNavigating;
        _currentFrame.Navigated -= CurrentFrameOnNavigated;

        _cts.Cancel();

        try
        {
            var target = _currentFrame.GetNavigationTarget();
            if (target is INavigationLeavingAware navigationAware)
            {
                await navigationAware.OnNavigatedFromAsync(CancellationToken.None);
            }
        }
        finally
        {
            _cts.Dispose();
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
        if (!_currentFrame.CanGoBack)
            throw new InvalidOperationException("Cannot navigate back because there is no page in the back stack.");

        _ = StartNavigation(
            _currentFrame.BackStack[^1].SourcePageType,
            NavigationKind.Navigate,
            static frame => frame.GoBack());
    }

    public void NavigateForward()
    {
        if (!_currentFrame.CanGoForward)
            throw new InvalidOperationException("Cannot navigate forward because there is no page in the forward stack.");

        _ = StartNavigation(
            _currentFrame.ForwardStack[^1].SourcePageType,
            NavigationKind.Navigate,
            static frame => frame.GoForward());
    }

    internal void Navigate(
        Type pageType,
        object? parameter = null,
        NavigationKind kind = NavigationKind.Navigate) =>
        _ = StartNavigation(pageType, parameter, kind);

    private void AttachChild(FrameNavigation child)
    {
        _child = child;
    }

    private void DetachChild(FrameNavigation child)
    {
        if (ReferenceEquals(_child, child))
        {
            _child = null;
        }
    }

    private async Task StartNavigation(Type pageType, object? parameter, NavigationKind kind)
    {
        EnsureCanOperate();
        NavigatorActivation.Push(_currentFrame);

        var source = _currentFrame.GetNavigationTarget();
        bool committed;

        try
        {
            if (!await CanNavigate())
                return;

            committed = NavigateTo(pageType, parameter, kind);
        }
        finally
        {
            NavigatorActivation.Pop(_currentFrame);
        }

        if (!committed)
            return;

        if (source is INavigationAware sourceAware)
        {
            await sourceAware.OnNavigatedFromAsync(_cts.Token);
        }
    }

    private async Task StartNavigation(Type pageType, NavigationKind kind, Action<Frame> action)
    {
        EnsureCanOperate();
        NavigatorActivation.Push(_currentFrame);

        var source = _currentFrame.GetNavigationTarget();

        try
        {
            if (!await CanNavigate())
                return;

            action.Invoke(_currentFrame);
        }
        finally
        {
            NavigatorActivation.Pop(_currentFrame);
        }

        if (source is INavigationLeavingAware sourceAware)
        {
            await sourceAware.OnNavigatedFromAsync(_cts.Token);
        }
    }

    private async Task<bool> CanNavigate()
    {
        if (_child is not null && !await _child.CanNavigate())
        {
            return false;
        }

        return await _currentFrame.CanNavigate(_cts.Token);
    }

    private bool NavigateTo(Type pageType, object? parameter, NavigationKind kind)
    {
        bool committed = _currentFrame.Navigate(
            pageType,
            parameter);

        if (!committed)
            return false;

        switch (kind)
        {
            case NavigationKind.Navigate:
                break;

            case NavigationKind.Replace:
                if (_currentFrame.BackStack.Count > 0)
                {
                    _currentFrame.BackStack.RemoveAt(
                        _currentFrame.BackStack.Count - 1);
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

    private void CurrentFrameOnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        
    }

    private async void CurrentFrameOnNavigated(object sender, NavigationEventArgs e)
    {
        await e.NotifyNavigatedToAsync(_cts.Token);
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
