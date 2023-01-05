using Microsoft.Extensions.Configuration;

namespace Templar
{
    internal class StaticSiteConfiguration
    {
        public string ContentRoot { get; }

        public string DefaultPage { get; }

        public StaticSiteConfiguration(IConfiguration configuration)
        {
            var localRoot = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
            var azureRoot = $@"{Environment.GetEnvironmentVariable("HOME")}\site\wwwroot";
            ContentRoot = Path.GetFullPath(Path.Combine(
                localRoot is null ? azureRoot : localRoot,
                configuration.GetValue("CONTENT_ROOT", "wwwroot")));
            DefaultPage = configuration.GetValue("DEFAULT_PAGE", "index.html");
        }
    }
}
