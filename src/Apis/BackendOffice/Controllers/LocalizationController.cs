using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Services.BackOffice.Application.Commands.Localization;
using TGS.Services.BackOffice.Application.Queries.Localization;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        [HttpPost("import/{systemCode}")]
        public async Task ImportLocalizationFileAsync(
            [FromServices] ICommandService<ImportLocalizationFileCommand> importService,
            string systemCode,
            [FromForm(Name = "file")] IFormFile file)
        {
            await using var fs = file.OpenReadStream();

            var command = new ImportLocalizationFileCommand(systemCode, fs);

            await importService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpGet("export/{systemCode}")]
        public async Task<FileContentResult> ExportLocalizationFileAsync(
            [FromServices] IGetService<ExportLocalizationFileCommand, Stream> exportService,
            string systemCode)
        {
            using var fs = await exportService
                .GetAsync(new ExportLocalizationFileCommand(systemCode))
                .ConfigureAwait(false);

            var content = new byte[fs.Length];

            await fs.ReadAsync(new Memory<byte>(content)).ConfigureAwait(false);

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
