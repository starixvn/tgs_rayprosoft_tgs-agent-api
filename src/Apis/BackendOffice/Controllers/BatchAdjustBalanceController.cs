using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.BrandService.Protos;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;
using TGS.Apis.BackendOffice.ViewModels;
using TGS.Core.Models.Enums;
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

    [HttpGet("{batchId}")]
    public IAsyncEnumerable<BatchAdjustBalanceItemViewModel> PreviewBatchDataAsync(
        [FromServices] IQueryService<PreviewBatchQuery, BatchAdjustBalanceItem> queryService,
        Guid batchId)
    {
        var query = new PreviewBatchQuery(batchId);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(item => new BatchAdjustBalanceItemViewModel
            {
                UserName = item.UserName,
                AdjustAmount = item.AdjustAmount,
                Turnover = item.Turnover,
                Remark = item.Remark
            });
    }

    [HttpPut("{brandId}/{batchId}")]
    public ValueTask ProcessBatchAsync(
        [FromServices] IMessageSender messageSender,
        Guid brandId,
        Guid batchId,
        [FromBody] PlayerAdjustmentTransactionType adjustmentTransactionType)
    {
        var request = new ProcessAdjustBalanceBatchRequest
        {
            BrandId = brandId,
            BatchId = batchId,
            AdjustTransferType = adjustmentTransactionType.ToString(),
            ModifyUserId = Guid.Parse(UserId)
        };

        return messageSender.PublishAsync(
            "tgs.bo.background.adjust_batch",
            request.ToByteArray(),
            HttpContext.RequestAborted);
    }
}
