using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navis.WinUI.Internal;

namespace Navis.WinUI.Extensions;

public static class NavigationServiceCollectionExtensions
{
    public static IServiceCollection AddNavisNavigation(this IServiceCollection services)
    {
        services.TryAddTransient<INavigation>(_ => NavigationFactory.Create());

        return services;
    }
}
