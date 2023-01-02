using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templar.Example.Components;
using Templar.Example.Middleware;
using Templar.Example.Services;

[assembly: FunctionsStartup(typeof(Templar.Example.Startup))]
namespace Templar.Example
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTemplar(o =>
            {
                o.AddComponent<HeaderComponent>()
                .AddComponent<FooterComponent>()
                .AddComponent<Test1Component>()
                .AddComponent<Test2Component>()
                .AddMiddleware<CacheControlMiddleware>()
                ;
            });
            builder.Services.AddScoped<TestService>();
        }
    }
}
