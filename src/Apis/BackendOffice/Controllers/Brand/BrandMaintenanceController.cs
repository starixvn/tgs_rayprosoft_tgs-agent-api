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
                .Select(b => new BrandMaintenanceResult
                {
                    BrandName = b.BrandName,
                    BrandCode = b.BrandCode,
                    LicenseeName = b.LicenseeName,
                    BeginTime = b.BeginTime,
                    EndTime = b.EndTime
                })
                .ToArrayAsync()
                .ConfigureAwait(false)
        };
    }

    [HttpPut(Name = "UpdateBrandMaintenance")]
    [Description("Update multiple brand maintenance")]
    public async Task<MessageResponse> UpdateBrandMaintenanceAsync(
        [FromBody] UpdateBrandMaintenanceRequest request,
        [FromServices] ICommandService<BatchUpdateBrandMaintenanceCommand> updateService)
    {
        var command = new BatchUpdateBrandMaintenanceCommand(
            request.BrandCodes,
            request.BeginTime,
            request.EndTime);

        await updateService.ExecuteAsync(command).ConfigureAwait(false);

        return CreateValidResponse<MessageResponse>();
    }
}
