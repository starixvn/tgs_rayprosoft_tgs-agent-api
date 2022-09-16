using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Commands.BatchAdjust;

namespace TGS.Apis.BackendOffice.Controllers;

[Route("api/batch-adj-balance")]
[ApiController]
public class BatchAdjustBalanceController : BaseContextController
{
    [HttpGet]
    public async Task<FileStreamResult> ExportFormTemplateAsync(
        [FromServices] IGetService<ExportBatchAdjustFormTemplateCommand, Stream> exportService)
        => File(
            await exportService.GetAsync(
                new(),
                HttpContext.RequestAborted).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
}
