using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private readonly static PhysicalFileProvider _FileProvider =
            new PhysicalFileProvider(Environment.CurrentDirectory);

        [HttpGet]
        public async Task<JsonDocument> GetLocalizeDictionaryAsync()
        {
            var currentCultureName = CultureInfo.CurrentUICulture.Name;
            var fileInfo = _FileProvider.GetFileInfo($"language.{currentCultureName}.json");

            if (!fileInfo.Exists)
                fileInfo = _FileProvider.GetFileInfo("language.json");

            using var fileStream = fileInfo.CreateReadStream();

            var jsonDoc = await JsonDocument.ParseAsync(fileStream).ConfigureAwait(false);

            return jsonDoc;
        }
    }
}
