using Microsoft.AspNetCore.Http;

namespace Templar.Middleware
{
    public interface ITemplarMiddleware
    {
        Task Invoke(HttpRequest req, TemplarComponent page, IMiddlewareBuilder next);
    }
}
