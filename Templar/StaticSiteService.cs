namespace Templar
{
    internal class StaticSiteService : IStaticSiteService
    {
        private readonly StaticSiteConfiguration _staticSiteConfiguration;
        public StaticSiteService(StaticSiteConfiguration staticSiteConfiguration)
        {
            _staticSiteConfiguration = staticSiteConfiguration;
        }
        public string GetFile(string? file, string route = "")
        {
            if (!string.IsNullOrWhiteSpace(route))
                file = $"{route}/{file}";
            var filePath = GetFilePath(file ?? "");
            if (File.Exists(filePath))
            {
                var result = File.ReadAllText(filePath);
                return result;
            }
            throw new FileNotFoundException();
        }
        private string GetFilePath(string pathValue)
        {
            string fullPath = Path.GetFullPath(Path.Combine(_staticSiteConfiguration.ContentRoot, pathValue));
            if (!IsInDirectory(_staticSiteConfiguration.ContentRoot, fullPath))
            {
                throw new ArgumentException("Invalid path");
            }
            if (Directory.Exists(fullPath))
            {
                fullPath = Path.Combine(fullPath, _staticSiteConfiguration.DefaultPage);
            }
            return fullPath;
        }

        private static bool IsInDirectory(string parentPath, string childPath) => childPath.StartsWith(parentPath);
    }
    public interface IStaticSiteService
    {
        string GetFile(string? file, string route = "");
    }
}
