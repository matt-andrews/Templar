using Microsoft.AspNetCore.Http;

namespace Templar.Middleware
{
    internal class MiddlewareBuilder : IMiddlewareBuilder
    {
        private readonly TemplarOptions _options;
        private int _index;
        private ITemplarMiddleware? _last;

        public MiddlewareBuilder(TemplarOptions options, IServiceProvider serviceProvider)
        {
            _options = options;
            _options.Middleware.AddServiceProvider(serviceProvider);
        }
        public async Task<Stream?> Invoke(HttpRequest req, TemplarComponent page)
        {
            await Next(req, page);
            if (_last is IMiddlewareResult result)
                return result.Result;
            throw new Exception("IMiddlewareResult was never reached. Did you call IMiddlewareBuilder.Next() from your custom middleware?");
        }
        public async Task Next(HttpRequest req, TemplarComponent page)
        {
            var next = _options.Middleware[_index++];
            if (next is not null)
            {
                _last = next;
                await next.Invoke(req, page, this);
            }
        }
    }
    public interface IMiddlewareBuilder
    {
        Task Next(HttpRequest req, TemplarComponent page);
    }
}
