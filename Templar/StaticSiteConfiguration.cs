using Microsoft.Extensions.Configuration;

namespace Templar
{
    internal class StaticSiteConfiguration
    {
        public string ContentRoot { get; }

        public string DefaultPage { get; }

        public StaticSiteConfiguration(IConfiguration configuration)
        {
            ContentRoot = Path.GetFullPath(Path.Combine(
                configuration.GetValue("AzureWebJobsScriptRoot", string.Empty),
                configuration.GetValue("CONTENT_ROOT", "wwwroot")));
            DefaultPage = configuration.GetValue("DEFAULT_PAGE", "index.html");
        }
    }
}
