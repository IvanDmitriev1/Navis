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
    private bool _commitObserved;
    private bool _isCommitting;
    private bool _isNavigating;

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
            return _currentFrame.CanGoForward;
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

        _ = StartJournalNavigation(NavigationMode.Back);
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

        if (_isNavigating)
            return;

        _isNavigating = true;

        try
        {
            var source = _currentFrame.GetNavigationTarget();

            if (!await CanNavigate())
                return;

            if (!NavigateTo(pageType, parameter, kind))
                return;

            var destination = _currentFrame.Content;
            await NotifyNavigationAsync(source, destination, parameter);
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async Task StartJournalNavigation(NavigationMode mode)
    {
        EnsureCanOperate();

        if (_isNavigating)
            return;

        _isNavigating = true;

        try
        {
            var source = _currentFrame.GetNavigationTarget();

            if (!await CanNavigate())
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

            var destination = _currentFrame.Content;
            await NotifyNavigationAsync(source, destination, parameter);
        }
        finally
        {
            _isNavigating = false;
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
        bool committed;
        _isCommitting = true;
        NavigatorActivation.Push(_currentFrame);

        try
        {
            committed = _currentFrame.CurrentSourcePageType == pageType || _currentFrame.Navigate(pageType, parameter);
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

    private async Task NotifyNavigationAsync(object? source, object? destination, object? parameter)
    {
        if (destination is not FrameworkElement { DataContext: { } destinationDataContext })
            return;

        if (!ReferenceEquals(source, destinationDataContext) && source is INavigationLeavingAware sourceAware)
        {
            await sourceAware.OnNavigatedFromAsync(_cts.Token);
        }

        await destination.NotifyNavigatedToAsync(parameter, _cts.Token);
    }

    private void CurrentFrameOnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        
    }

    private async void CurrentFrameOnNavigated(object sender, NavigationEventArgs e)
    {
        if (_isCommitting)
        {
            _commitObserved = true;
            return;
        }

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
