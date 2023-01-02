using Microsoft.AspNetCore.Mvc;
using MimeTypes;

namespace Templar
{
    internal class StaticSiteService : IStaticSiteService
    {
        private readonly StaticSiteConfiguration _staticSiteConfiguration;
        public StaticSiteService(StaticSiteConfiguration staticSiteConfiguration)
        {
            _staticSiteConfiguration = staticSiteConfiguration;
        }
        public Task<IActionResult> Run(string? file, string route = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(route))
                {
                    file = $"{route}/{file}";
                }
                var filePath = GetFilePath(file ?? "");
                if (File.Exists(filePath))
                {
                    var stream = File.OpenRead(filePath);
                    return Task.FromResult<IActionResult>(new FileStreamResult(stream, GetMimeType(filePath))
                    {
                        LastModified = File.GetLastWriteTime(filePath)
                    });
                }
                else
                {
                    return Task.FromResult<IActionResult>(new NotFoundResult());
                }
            }
            catch
            {
                return Task.FromResult<IActionResult>(new BadRequestResult());
            }
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

        private static string GetMimeType(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return MimeTypeMap.GetMimeType(fileInfo.Extension);
        }
    }
    public interface IStaticSiteService
    {
        Task<IActionResult> Run(string? file, string route = "");
        string GetFile(string? file, string route = "");
    }
}
