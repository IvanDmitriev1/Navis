using System.Diagnostics;

namespace Navis.WinUI.Internal;

internal sealed class FrameNavigation : INavigation, IDisposable
{
    public FrameNavigation(Frame currentFrame, INavigation? parent)
    {
        _currentFrame = currentFrame;
        Parent = parent;

        currentFrame.Navigating += OnFrameNavigating;
        currentFrame.Navigated += OnFrameNavigated;
    }

    private readonly Frame _currentFrame;
    private bool _disposed;


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

    public INavigation? Parent { get; }

    public Type? CurrentPageType => _currentFrame.CurrentSourcePageType;

    public bool CanGoBack => _currentFrame.CanGoBack;
    public bool CanGoForward => _currentFrame.CanGoForward;

    public void Dispose()
    {
        if (_disposed)
            return;

        _currentFrame.Navigating -= OnFrameNavigating;
        _currentFrame.Navigated -= OnFrameNavigated;
        _disposed = true;
    }

    public void NavigateBack()
    {
        NavigatorActivation.Push(_currentFrame);

        try
        {
            _currentFrame.GoBack();
        }
        finally
        {
            NavigatorActivation.Pop(_currentFrame);
        }
    }

    public void NavigateForward()
    {
        NavigatorActivation.Push(_currentFrame);

        try
        {
            _currentFrame.GoForward();
        }
        finally
        {
            NavigatorActivation.Pop(_currentFrame);
        }
    }

    public void Navigate<TPage>(NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page
    {
        Navigate(typeof(TPage), null, kind);
    }

    public void Navigate<TPage, TParameter>(TParameter parameter, NavigationKind kind = NavigationKind.Navigate)
        where TPage : Page
    {
        Navigate(typeof(TPage), parameter, kind);
    }

    public void Navigate(Type pageType, object? parameter = null, NavigationKind kind = NavigationKind.Navigate)
    {
        switch (kind)
        {
            case NavigationKind.Navigate:
                NavigateTo(pageType, parameter);
                return;

            case NavigationKind.Replace:
                if (NavigateTo(pageType, parameter) && _currentFrame.BackStack.Count > 0)
                    _currentFrame.BackStack.RemoveAt(_currentFrame.BackStack.Count - 1);
                return;

            case NavigationKind.Reset:
                if (NavigateTo(pageType, parameter))
                {
                    _currentFrame.BackStack.Clear();
                    _currentFrame.ForwardStack.Clear();
                }
                return;

            case NavigationKind.Back:
            case NavigationKind.Forward:
                throw new ArgumentException(null, nameof(kind));

            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }


    private bool NavigateTo(Type pageType, object? parameter)
    {
        NavigatorActivation.Push(_currentFrame);

        try
        {
            var result = _currentFrame.Navigate(pageType, parameter);
            Debug.Assert(result);
            return result;
        }
        finally
        {
            NavigatorActivation.Pop(_currentFrame);
        }
    }

    private void OnFrameNavigating(object sender, NavigatingCancelEventArgs args)
    {

    }

    private void OnFrameNavigated(object sender, NavigationEventArgs args)
    {

    }
}
