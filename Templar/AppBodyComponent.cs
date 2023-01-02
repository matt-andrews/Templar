using Microsoft.AspNetCore.Http;
using System.Reflection;
using Templar.Containers;

namespace Templar
{
    internal class AppBodyComponent : TemplarComponent
    {
        public override string TemplatePath { get; }
        public AppBodyComponent(string baseTemplatePath)
        {
            TemplatePath = baseTemplatePath;
        }
        protected override string GetFile(IStaticSiteService siteService)
        {
            var file = base.GetFile(siteService);
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
            file = file.Replace("<!DOCTYPE html>", $"<!DOCTYPE html>\n<!--Generated with Templar v{version} https://github.com/matt-andrews/Templar-->");
            return file;
        }
        public async Task<string> Initialize(
            TemplarComponent page,
            IComponents components,
            IStaticSiteService siteService,
            IServiceProvider serviceProvider,
            IParameters routeParameters,
            HttpRequest req
            )
        {
            var file = GetFile(siteService);
            file = await page.Initialize(file, "{Component:AppBody}", components, siteService, serviceProvider, routeParameters, req);
            return await BuildComponents(file, components, siteService, serviceProvider, routeParameters, req);
        }
    }
}
