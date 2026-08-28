using System.Diagnostics;

namespace Navis.WinUI.Internal;

internal sealed class FrameNavigation(Frame currentFrame, INavigation? parent) : INavigation
{

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

    public INavigation? Parent => parent;

    public Type? CurrentPageType => currentFrame.CurrentSourcePageType;

    public bool CanGoBack => currentFrame.CanGoBack;
    public bool CanGoForward => currentFrame.CanGoForward;

    public void Navigate(NavigationKind kind)
    {
        if (kind is not NavigationKind.Back and not NavigationKind.Forward)
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }

        NavigatorActivation.Push(currentFrame);

        try
        {
            if (kind == NavigationKind.Back)
                currentFrame.GoBack();
            else
                currentFrame.GoForward();
        }
        finally
        {
            NavigatorActivation.Pop(currentFrame);
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
                if (NavigateTo(pageType, parameter) && currentFrame.BackStack.Count > 0)
                    currentFrame.BackStack.RemoveAt(currentFrame.BackStack.Count - 1);
                return;

            case NavigationKind.Reset:
                if (NavigateTo(pageType, parameter))
                {
                    currentFrame.BackStack.Clear();
                    currentFrame.ForwardStack.Clear();
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
        NavigatorActivation.Push(currentFrame);

        try
        {
            var result = currentFrame.Navigate(pageType, parameter);
            Debug.Assert(result);
            return result;
        }
        finally
        {
            NavigatorActivation.Pop(currentFrame);
        }
    }
}