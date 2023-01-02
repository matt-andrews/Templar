using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Templar.Middleware;

namespace Templar.Example.Middleware
{
    internal class CacheControlMiddleware : ITemplarMiddleware
    {
        public async Task Invoke(HttpRequest req, TemplarComponent page, IMiddlewareBuilder next)
        {
            if (page.TemplatePath.EndsWith(".html"))
            {
                req.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            }
            await next.Next(req, page);
        }
    }
}
