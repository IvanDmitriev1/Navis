using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navis.WinUI.Internal;

namespace Navis.WinUI.Extensions;

public static class NavigationServiceCollectionExtensions
{
    public static IServiceCollection AddNavisNavigation(this IServiceCollection services)
    {
        services.TryAddTransient<INavigation>(sp =>
        {
            var currentFrame = NavigatorActivation.CurrentFrame;
            return NavigationHost.GetFrameNavigationInstance(currentFrame);
        });

        return services;
    }
}
