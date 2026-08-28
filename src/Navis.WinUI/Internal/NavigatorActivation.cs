namespace Navis.WinUI.Internal;

internal static class NavigatorActivation
{
    private static readonly Stack<Frame> Frames = [];

    public static Frame CurrentFrame =>
        Frames.Count != 0
            ? Frames.Peek()
            : throw new InvalidOperationException(
                "INavigator can only be resolved while a Page is being activated by a Navis Frame.");

    public static void Push(Frame frame)
    {
        Frames.Push(frame);
    }

    public static void Pop(Frame frame)
    {
        if (Frames.Count == 0 ||
            !ReferenceEquals(Frames.Peek(), frame))
        {
            throw new InvalidOperationException(
                "Invalid navigation activation stack.");
        }

        Frames.Pop();
    }
}