using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Services.BackOffice.Application.Commands.Localization;
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
            using var fs = file.OpenReadStream();

            var command = new ImportLocalizationFileCommand(systemCode, fs);

            await importService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
