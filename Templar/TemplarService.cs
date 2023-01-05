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
            var result = await _middleware.Invoke(req, page);
            if (result is not null)
                return new FileStreamResult(result, page.MimeType);
            return new BadRequestResult();
        }
        public async Task<IActionResult> Invoke(HttpRequest req, string folder = "", string file = "")
        {
            var page = new StaticContentComponent($"{folder}/{file}");
            var result = await _middleware.Invoke(req, page);
            if (result is not null)
                return new FileStreamResult(result, page.MimeType);
            return new BadRequestResult();
        }
    }
    public interface ITemplarService
    {
        Task<IActionResult> Invoke(HttpRequest req, TemplarComponent page);
        Task<IActionResult> Invoke(HttpRequest req, string folder = "", string file = "");
    }
}
