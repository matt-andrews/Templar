using Microsoft.Extensions.DependencyInjection;
using Templar.Containers;
using Templar.Middleware;

namespace Templar
{
    internal class TemplarOptions : ITemplarOptions
    {
        public IComponents Components { get; } = new ComponentsContainer();
        public string ContentRoot { get; set; } = "index.html";
        public IMiddlewareContainer Middleware { get; } = new MiddlewareContainer();
        public IServiceCollection ServiceCollection { get; }
        public TemplarOptions(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }
    }
    public static class TemplarOptionsExtensions
    {
        public static ITemplarOptions AddComponent<T>(this ITemplarOptions options)
            where T : TemplarComponent
        {
            options.Components.Add<T>();
            return options;
        }
        public static ITemplarOptions AddMiddleware<T>(this ITemplarOptions options)
            where T : class, ITemplarMiddleware
        {
            options.Middleware.Add<T>();
            options.ServiceCollection.AddTransient<T>();
            return options;
        }
    }
    public interface ITemplarOptions
    {
        IComponents Components { get; }
        string ContentRoot { get; set; }
        IMiddlewareContainer Middleware { get; }
        IServiceCollection ServiceCollection { get; }
    }
}
