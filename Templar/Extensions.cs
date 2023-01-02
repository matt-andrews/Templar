using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using Templar.Containers;
using Templar.Middleware;

namespace Templar
{
    public static class Extensions
    {
        public static IServiceCollection AddTemplar(this IServiceCollection services, Action<ITemplarOptions> options)
        {
            var templarOptions = new TemplarOptions(services);
            options.Invoke(templarOptions);
            templarOptions.AddMiddleware<PageBuilderMiddleware>();
            return services
                .AddScoped<MiddlewareBuilder>()
                .AddScoped<ITemplarService, TemplarService>()
                .AddScoped<IStaticSiteService, StaticSiteService>()
                .AddSingleton<StaticSiteConfiguration>()
                .AddSingleton(templarOptions)
                ;
        }
    }
}
