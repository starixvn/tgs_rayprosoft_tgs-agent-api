using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.BackendOffice.Controllers.Messages.Player;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Models.ReportPlayer;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerIdentityController : BaseContextController
    {
        [HttpGet("{playerCode}/identity", Name = "GetPlayerIdentityAuthenticationForm")]
        [Description("Get a player identity authentication form")]
        public async ValueTask<PlayerIdentityForm> GetPlayerIdentityAuthenticationFormAsync(
            [FromServices] IGetService<GetPlayerIdentityFormQuery, PlayerIdentityInfo> getService,
            string playerCode)
        {
            var query = new GetPlayerIdentityFormQuery(playerCode);

            var info = await getService.GetAsync(query).ConfigureAwait(false);

            return new PlayerIdentityForm
            {
                FormId = info.Id,
                RealName = info.RealName,
                IdNumber = info.IdNumber,
                AuthData = JsonDocument.Parse(info.Data),
                CreatedOn = info.CreatedOn,
                Status = info.Status,
                Remark = info.Remark,
                ReviewedOn = info.ReviewedOn,
                ReviewedUserName = info.ReviewedUserName
            };
        }

        [HttpGet(Name = "GetPlayerIdentityAuthenticationForms")]
        [Description("Get player identity authentication forms")]
        public async Task<PagedResponse<PlayerIdentityAuthenticationsModel>> GetPlayerIdentityAuthenticationFormsAsync(
            [FromServices] IGetService<GetPlayerIdentityAuthenticationFormsQuery, int> countService,
            [FromServices] IQueryService<GetPlayerIdentityAuthenticationFormsQuery, PlayerIdentityAuthenticationInfo> queryService,
            [FromQuery] GetPlayerIdentityAuthenticationListRequest request,
            [FromQuery] PageableRequest pagingRequest)
        {
            var paging = new Paging<PlayerIdentityAuthenticationInfo>
            {
                RowCount = pagingRequest.Rows,
                Page = pagingRequest.Page
            };

            paging.SortByPropertyName(
                string.IsNullOrWhiteSpace(pagingRequest.SortBy)
                    ? nameof(PlayerIdentityAuthenticationInfo.CreatedOn)
                    : pagingRequest.SortBy,
                pagingRequest.IsDesc);

            var query = new GetPlayerIdentityAuthenticationFormsQuery(
                request.BrandId,
                request.Status,
                request.Username,
                request.BeginCreatedOn,
                request.EndCreatedOn,
                request.BeginReviewedOn,
                request.EndReviewedOn,
                paging);

            return new PagedResponse<PlayerIdentityAuthenticationsModel>
            {
                Valid = true,
                TotalCount = await countService
                    .GetAsync(query)
                    .ConfigureAwait(false),
                Items = await queryService.QueryAsync(query)
                    .Select(info => new PlayerIdentityAuthenticationsModel
                    {
                        IdNumber = info.IdNumber,
                        RealName = info.RealName,
                        Remark = info.Remark,
                        Status = info.Status,
                        Username = info.Username,
                        Data = string.IsNullOrEmpty(info.Data)
                            ? JsonDocument.Parse("{}")
                            : JsonDocument.Parse(info.Data),
                        FormId = info.PlayerAuthenticationFormId,
                        PlayerCode = info.PlayerCode
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpPost("{formId}", Name = "ReviewPlayerIdentityForm")]
        public async Task<MessageResponse> ReviewPlayerIdentityFormAsync(
            [FromServices] ICommandService<ReviewPlayerIdentityFormCommand> reviewCommandService,
            Guid formId,
            [FromBody] PlayerIdentityReviewRequest reviewResponse)
        {
            var command = new ReviewPlayerIdentityFormCommand(
                reviewResponse.BrandId,
                formId,
                reviewResponse.ReviewResponse.ToReviewState(),
                reviewResponse.Remark,
                UserId,
                Username,
                reviewResponse.RealName,
                reviewResponse.IdNumber);

            await reviewCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
