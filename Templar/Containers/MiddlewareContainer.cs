using Templar.Middleware;

namespace Templar.Containers
{
    internal class MiddlewareContainer : IMiddlewareContainer
    {
        private IServiceProvider? _serviceProvider;
        private readonly List<Type> _adapters = new();

        public void Add<T>()
            where T : class, ITemplarMiddleware
        {
            _adapters.Add(typeof(T));
        }
        public ITemplarMiddleware? this[int index]
        {
            get
            {
                if (_serviceProvider is null)
                    throw new Exception("ServiceProvider has not been initialized");
                var find = _adapters.ElementAtOrDefault(index);
                if (find is null)
                    return null;
                return _serviceProvider.GetService(find) as ITemplarMiddleware;
            }
        }
        public void AddServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
    public interface IMiddlewareContainer
    {
        void Add<T>() where T : class, ITemplarMiddleware;
        ITemplarMiddleware? this[int index] { get; }
        void AddServiceProvider(IServiceProvider serviceProvider);
    }
}
