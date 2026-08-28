namespace Navis.WinUI.Abstractions;

public sealed record NavigationContext(
    NavigationKind Kind,
    Type? SourcePageType,
    Type DestinationPageType,
    object? Parameter)
{
    public TParameter GetParameter<TParameter>()
    {
        if (Parameter is TParameter typedParameter)
            return typedParameter;

        throw new InvalidOperationException(
            $"The navigation parameter value cannot be read as " +
            $"'{typeof(TParameter).FullName}'.");
    }
}
