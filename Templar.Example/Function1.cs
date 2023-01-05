using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Templar.Example.Components;

namespace Templar.Example
{
    public class Function1
    {
        private readonly ITemplarService _templarService;
        public Function1(ITemplarService templarService)
        {
            _templarService = templarService;
        }

        [FunctionName("StaticRoot")]
        public async Task<IActionResult> StaticRoot(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{file?}")]
            HttpRequest req)
        {
            return await _templarService.Invoke(req, new IndexPage());
        }
        [FunctionName("StaticContent")]
        public async Task<IActionResult> StaticContent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "content/{folder}/{file}")] HttpRequest req,
            string file, string folder)
        {
            return await _templarService.Invoke(req, folder, file);
        }
    }
}
