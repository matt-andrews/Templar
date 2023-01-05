using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text;
using Templar.Containers;

namespace Templar.Middleware
{
    internal class PageBuilderMiddleware : ITemplarMiddleware, IMiddlewareResult
    {
        public Stream? Result { get; private set; }
        private readonly IStaticSiteService _siteService;
        private readonly IServiceProvider _serviceProvider;
        private readonly TemplarOptions _options;
        public PageBuilderMiddleware(IStaticSiteService siteService, IServiceProvider serviceProvider, TemplarOptions options)
        {
            _siteService = siteService;
            _serviceProvider = serviceProvider;
            _options = options;
        }
        public async Task Invoke(HttpRequest req, TemplarComponent page, IMiddlewareBuilder next)
        {
            string file;
            if (page is StaticContentComponent staticContent)
            {
                file = staticContent.GetStaticFile(_siteService);
            }
            else
            {
                var appBody = new AppBodyComponent(_options.ContentRoot);
                var parameters = BuildParameters(req);
                file = await appBody.Initialize(page, _options.Components, _siteService, _serviceProvider, parameters, req);
            }
            byte[] byteArray = Encoding.UTF8.GetBytes(file);
            Result = new MemoryStream(byteArray);
        }
        private static ParametersContainer BuildParameters(HttpRequest req)
        {
            var parameters = new ParametersContainer();
            var routes = req.HttpContext.GetRouteData();
            foreach (var r in routes.Values)
            {
                parameters.Add(r.Key, r.Value.ToString() ?? "");
            }
            foreach (var p in req.Query)
            {
                parameters.Add(p.Key, p.Value);
            }
            return parameters;
        }
    }
}
