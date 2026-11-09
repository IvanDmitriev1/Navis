using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Navis.WinUI.Sample.Messages;

public enum SampleScreen
{
    Shell,
    Standalone
}

public sealed class ScreenChangeRequestedMessage(SampleScreen value) : ValueChangedMessage<SampleScreen>(value)
{
}
