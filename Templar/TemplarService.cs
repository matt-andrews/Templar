using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Templar.Middleware;

namespace Templar
{
    internal class TemplarService : ITemplarService
    {
        private readonly MiddlewareBuilder _middleware;
        public TemplarService(MiddlewareBuilder middleware)
        {
            _middleware = middleware;
        }

        public async Task<IActionResult> Invoke(HttpRequest req, TemplarComponent page)
        {
            var file = await _middleware.Invoke(req, page);
            if (file is not null)
                return new FileStreamResult(file, "text/html");
            return new BadRequestResult();
        }
    }
    public interface ITemplarService
    {
        Task<IActionResult> Invoke(HttpRequest req, TemplarComponent page);
    }
}
