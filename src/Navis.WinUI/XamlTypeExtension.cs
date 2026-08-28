using Microsoft.UI.Xaml.Markup;

namespace Navis.WinUI;

[MarkupExtensionReturnType(ReturnType = typeof(Type))]
public sealed class XamlTypeExtension : MarkupExtension
{
    public string TypeName { get; set; } = string.Empty;

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        if (string.IsNullOrWhiteSpace(TypeName))
            throw new InvalidOperationException("XamlType.TypeName cannot be empty.");

        if (serviceProvider.GetService(typeof(IXamlTypeResolver)) is not IXamlTypeResolver resolver)
            throw new InvalidOperationException("The WinUI XAML type resolver is unavailable.");

        return resolver.Resolve(TypeName) ?? throw new InvalidOperationException(
            $"XAML type '{TypeName}' could not be resolved.");
    }
}
