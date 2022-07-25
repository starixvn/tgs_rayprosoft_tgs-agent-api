using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandMaintenance;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers.Brand;

[Route("api/[controller]")]
[ApiController]
public class BrandMaintenanceController : BaseContextController
{
    [HttpPut("batchupdatetime", Name = "BatchUpdateBrandMaintenance")]
    [Description("Batch Update multiple brand maintenance")]
    public async Task<MessageResponse> BatchUpdateBrandMaintenanceAsync(
        [FromBody] BatchUpdateBrandMaintenanceRequest request,
        [FromServices] ICommandService<BatchUpdateBrandMaintenanceCommand> updateService)
    {
        var command = new BatchUpdateBrandMaintenanceCommand(
            request.BrandIds,
            request.BeginTime,
            request.EndTime);

        await updateService.ExecuteAsync(command).ConfigureAwait(false);

        return CreateValidResponse<MessageResponse>();
    }

    [HttpGet(Name = "GetAllBrandMaintenance")]
    [Description("Get all brand maintenance")]
    public async Task<PagedResponse<BrandMaintenanceResult>> GetAllBrandMaintenanceAsync(
        [FromServices] IGetService<GetAllBrandMaintenanceQuery, int> countService,
        [FromServices] IQueryService<GetAllBrandMaintenanceQuery, BrandMaintenanceInfo> queryService,
        [FromQuery] AllBrandMaintenanceRequest request,
        [FromQuery] PageableRequest pagingRequest)
    {
        var paging = new Paging<BrandMaintenanceInfo>
        {
            RowCount = pagingRequest.Rows,
            Page = pagingRequest.Page
        };

        paging.SortByPropertyName(
            string.IsNullOrWhiteSpace(pagingRequest.SortBy)
                ? nameof(BrandMaintenanceInfo.BrandCode)
                : pagingRequest.SortBy,
            pagingRequest.IsDesc);

        var query = new GetAllBrandMaintenanceQuery(
            request.BrandCode,
            paging);

        return new PagedResponse<BrandMaintenanceResult>
        {
            Valid = true,
            TotalCount = await countService
                .GetAsync(query)
                .ConfigureAwait(false),
            Items = await queryService.QueryAsync(query)
                .Select(bmi => new BrandMaintenanceResult
                {
                    BrandName = bmi.BrandName,
                    BrandCode = bmi.BrandCode,
                    LicenseeName = bmi.LicenseeName,
                    BeginTime = bmi.BeginTime,
                    EndTime = bmi.EndTime,
                    Id = bmi.Id,
                })
                .ToArrayAsync()
                .ConfigureAwait(false)
        };
    }

    [HttpGet("{Id}", Name = "GetBrandMaintenance")]
    [Description("Get a brand maintenance")]
    public async Task<BrandMaintenanceViewModel> GetBrandMaintenanceAsync(
        Guid Id,
        [FromServices] IGetService<GetBrandMaintenanceQuery, BrandMaintenanceModel> getService)
    {
        var query = new GetBrandMaintenanceQuery(Id);

        var brandMaintenance = await getService
            .GetAsync(query)
            .ConfigureAwait(false);

        return new BrandMaintenanceViewModel
        {
            Content = brandMaintenance.Content
        };
    }

    [HttpPut("{Id}", Name = "UpdateBrandMaintenance")]
    [Description("Update a brand maintenance's content")]
    public async Task<MessageResponse> UpdateBrandMaintenanceAsync(
        Guid Id,
        [FromBody] UpdateBrandMaintenanceRequest request,
        [FromServices] ICommandService<UpdateBrandMaintenanceCommand> updateService)
    {
        var command = new UpdateBrandMaintenanceCommand(
            Id,
            request.Content);

        await updateService.ExecuteAsync(command).ConfigureAwait(false);

        return CreateValidResponse<MessageResponse>();
    }
}
