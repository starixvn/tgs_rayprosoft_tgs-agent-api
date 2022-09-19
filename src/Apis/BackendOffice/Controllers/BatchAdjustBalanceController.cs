using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Services.BatchAdjust;
using TGS.Services.BackOffice.Domain;

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

    [HttpPost("{brandId}")]
    public async Task<Guid> ImportBatchAsync(
        [FromServices] IQueryService<ReadBatchAdjustItemsQuery, BatchAdjustBalanceItem> readService,
        [FromServices] IGetService<SaveBatchCommand, Guid> saveService,
        Guid brandId,
        [FromForm(Name = "file")] IFormFile file)
    {
        await using var fs = file.OpenReadStream();

        var query = new ReadBatchAdjustItemsQuery(brandId, fs);

        var items = readService.QueryAsync(query, HttpContext.RequestAborted);

        return await saveService
            .GetAsync(new(items), HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }
}
