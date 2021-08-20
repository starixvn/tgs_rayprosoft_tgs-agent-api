using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TGS.Apis.BackendOffice.Controllers.Messages.Banner;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : BaseContextController
    {
        [HttpPut("{brandId}")]
        public ValueTask CreateBannerAsync(
            [FromServices] ICommandService<CreateBannerCommand> createService,
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            Guid brandId,
            [FromBody] BannerModifyRequest request)
        {
            var jsonOptions = jsonOptionsAccessor.Value;

            var command = new CreateBannerCommand(
                brandId,
                request.Title,
                request.BeginTime,
                request.EndTime,
                jsonSerializationProvider.SerializeToString(
                    request.Localization,
                    jsonOptions.JsonSerializerOptions),
                this.Username);

            return createService.ExecuteAsync(command);
        }

        [HttpPatch("disable/{brandId}/{bannerId}")]
        public ValueTask DisableBannerAsync(
            [FromServices] ICommandService<SetIsEanbleBannerCommand> setService,
            Guid brandId,
            Guid bannerId)
        {
            var command = new SetIsEanbleBannerCommand(
                brandId,
                bannerId,
                false,
                Username);

            return setService.ExecuteAsync(command);
        }

        [HttpPatch("enable/{brandId}/{bannerId}")]
        public ValueTask EnableBannerAsync(
            [FromServices] ICommandService<SetIsEanbleBannerCommand> setService,
            Guid brandId,
            Guid bannerId)
        {
            var command = new SetIsEanbleBannerCommand(
                brandId,
                bannerId,
                true,
                Username);

            return setService.ExecuteAsync(command);
        }

        [HttpGet("expired/{brandId}")]
        public async Task<PagedResponse<BannerListItem>> GetExpiredBannersAsync(
            [FromServices] IGetService<GetBannersQuery, int> countService,
            [FromServices] IQueryService<GetBannersQuery, BannerInfo> queryService,
            Guid brandId,
            [FromQuery] PageableRequest pagingRequest)
        {
            var paging = new Paging.Paging<BannerInfo>();

            paging.RowCount = pagingRequest.Rows;
            paging.Page = pagingRequest.Page;

            paging.SortByPropertyName(
                string.IsNullOrWhiteSpace(pagingRequest.SortBy)
                    ? nameof(BannerInfo.DisplayOrder)
                    : pagingRequest.SortBy,
                pagingRequest.IsDesc);

            var query = new GetBannersQuery(
                brandId,
                BannerStatus.Expired,
                paging);

            return new PagedResponse<BannerListItem>
            {
                Valid = true,
                TotalCount = await countService
                    .GetAsync(query)
                    .ConfigureAwait(false),
                Items = await queryService.QueryAsync(query)
                    .Select(b => new BannerListItem
                    {
                        Id = b.Id,
                        BeginTime = b.BeginTime,
                        EndTime = b.EndTime,
                        Modifier = b.Updater,
                        Status = b.Status,
                        Title = b.Title,
                        UpdatedOn = b.UpdatedOn,
                        IsEnabled = b.IsEnabled,
                        DisplayOrder = b.DisplayOrder
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpGet("rejected/{brandId}")]
        public async Task<PagedResponse<BannerListItem>> GetRejectedBannersAsync(
            [FromServices] IGetService<GetBannersQuery, int> countService,
            [FromServices] IQueryService<GetBannersQuery, BannerInfo> queryService,
            Guid brandId,
            [FromQuery] PageableRequest pagingRequest)
        {
            var paging = new Paging.Paging<BannerInfo>();

            paging.RowCount = pagingRequest.Rows;
            paging.Page = pagingRequest.Page;

            paging.SortByPropertyName(
                string.IsNullOrWhiteSpace(pagingRequest.SortBy)
                    ? nameof(BannerInfo.DisplayOrder)
                    : pagingRequest.SortBy,
                pagingRequest.IsDesc);

            var query = new GetBannersQuery(
                brandId,
                BannerStatus.Expired,
                paging);

            return new PagedResponse<BannerListItem>
            {
                Valid = true,
                TotalCount = await countService
                    .GetAsync(query)
                    .ConfigureAwait(false),
                Items = await queryService.QueryAsync(query)
                    .Select(b => new BannerListItem
                    {
                        Id = b.Id,
                        BeginTime = b.BeginTime,
                        EndTime = b.EndTime,
                        Modifier = b.Updater,
                        Status = b.Status,
                        Title = b.Title,
                        UpdatedOn = b.UpdatedOn,
                        IsEnabled = b.IsEnabled,
                        DisplayOrder = b.DisplayOrder
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpGet("reviewing/{brandId}")]
        public async Task<PagedResponse<BannerListItem>> GetReviewingBannersAsync(
            [FromServices] IGetService<GetBannersQuery, int> countService,
            [FromServices] IQueryService<GetBannersQuery, BannerInfo> queryService,
            Guid brandId,
            [FromQuery] PageableRequest pagingRequest)
        {
            var paging = new Paging.Paging<BannerInfo>();

            paging.RowCount = pagingRequest.Rows;
            paging.Page = pagingRequest.Page;

            paging.SortByPropertyName(
                string.IsNullOrWhiteSpace(pagingRequest.SortBy)
                    ? nameof(BannerInfo.DisplayOrder)
                    : pagingRequest.SortBy,
                pagingRequest.IsDesc);

            var query = new GetBannersQuery(
                brandId,
                BannerStatus.Reviewing,
                paging);

            return new PagedResponse<BannerListItem>
            {
                Valid = true,
                TotalCount = await countService
                    .GetAsync(query)
                    .ConfigureAwait(false),
                Items = await queryService.QueryAsync(query)
                    .Select(b => new BannerListItem
                    {
                        Id = b.Id,
                        BeginTime = b.BeginTime,
                        EndTime = b.EndTime,
                        Modifier = b.Updater,
                        Status = b.Status,
                        Title = b.Title,
                        UpdatedOn = b.UpdatedOn,
                        IsEnabled = b.IsEnabled,
                        DisplayOrder = b.DisplayOrder
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpGet("valid/{brandId}")]
        public async Task<PagedResponse<BannerListItem>> GetValidBannersAsync(
            [FromServices] IGetService<GetBannersQuery, int> countService,
            [FromServices] IQueryService<GetBannersQuery, BannerInfo> queryService,
            Guid brandId,
            [FromQuery] PageableRequest pagingRequest)
        {
            var paging = new Paging.Paging<BannerInfo>();

            paging.RowCount = pagingRequest.Rows;
            paging.Page = pagingRequest.Page;

            paging.SortByPropertyName(
                string.IsNullOrWhiteSpace(pagingRequest.SortBy)
                    ? nameof(BannerInfo.DisplayOrder)
                    : pagingRequest.SortBy,
                pagingRequest.IsDesc);

            var query = new GetBannersQuery(
                brandId,
                BannerStatus.Valid,
                paging);

            return new PagedResponse<BannerListItem>
            {
                Valid = true,
                TotalCount = await countService
                    .GetAsync(query)
                    .ConfigureAwait(false),
                Items = await queryService.QueryAsync(query)
                    .Select(b => new BannerListItem
                    {
                        Id = b.Id,
                        BeginTime = b.BeginTime,
                        EndTime = b.EndTime,
                        Modifier = b.Updater,
                        Status = b.Status,
                        Title = b.Title,
                        UpdatedOn = b.UpdatedOn,
                        IsEnabled = b.IsEnabled,
                        DisplayOrder = b.DisplayOrder
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpPost("{brandId}/{bannerId}")]
        public ValueTask ReviewBannerAsync(
            [FromServices] ICommandService<ReviewBannerCommand> reviewService,
            Guid brandId,
            Guid bannerId,
            [FromBody] ReviewBannerRequest request)
        {
            var command = new ReviewBannerCommand(
                brandId,
                bannerId,
                request.ReviewState,
                request.Remark,
                Username);

            return reviewService.ExecuteAsync(command);
        }

        [HttpPatch("{brandId}/{bannerId}")]
        public ValueTask UpdateBannerAsync(
            [FromServices] ICommandService<UpdateBannerCommand> updateService,
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            Guid brandId,
            Guid bannerId,
            [FromBody] BannerModifyRequest request)
        {
            var jsonOptions = jsonOptionsAccessor.Value;

            var command = new UpdateBannerCommand(
                brandId,
                bannerId,
                request.Title,
                request.BeginTime,
                request.EndTime,
                jsonSerializationProvider.SerializeToString(
                    request.Localization,
                    jsonOptions.JsonSerializerOptions),
                Username);

            return updateService.ExecuteAsync(command);
        }
    }
}
