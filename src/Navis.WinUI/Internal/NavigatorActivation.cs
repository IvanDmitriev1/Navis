namespace Navis.WinUI.Internal;

internal static class NavigatorActivation
{
    [ThreadStatic]
    private static Stack<Frame>? _frames;

    private static Stack<Frame> Frames => _frames ??= new Stack<Frame>();

    public static bool TryGetCurrentFrame(out Frame? frame)
    {
        frame = Frames.Count != 0 ? Frames.Peek() : null;
        return frame is not null;
    }

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
