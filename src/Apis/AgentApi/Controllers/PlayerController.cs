using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Player;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpGet("list")]
        [Description("Get players of agent")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<PagedResponse<PlayerInfoModel>> GetPlayerInfosAsync(
            [FromServices] IQueryService<GetAgentPlayerInfosQuery, AgentPlayerInfo> queryService,
            [FromQuery] GetAgentPlayersRequest request,
            [FromQuery] PagingRequest pagingRequest)
        {
            var agentToken = HttpContext.Items.GetAgentTokenData();

            var query = new GetAgentPlayerInfosQuery(
                agentToken.AgentId,
                request.Username,
                request.BeginRegisteredOn,
                request.EndRegisteredOn,
                request.RegisterHost,
                request.Status,
                request.VipId);

            var playerInfos = queryService.QueryAsync(query);

            var paging = new Paging<PlayerInfoModel>();

            if (!string.IsNullOrWhiteSpace(pagingRequest.SortBy))
            {
                paging.SortByPropertyName(pagingRequest.SortBy, paging.IsDesc);
            }

            if (pagingRequest.ShowAll)
            {
                return new PagedResponse<PlayerInfoModel>
                {
                    TotalCount = await playerInfos.CountAsync().ConfigureAwait(false),
                    Items = await playerInfos
                        .Select(playerInfo => new PlayerInfoModel
                        {
                            AreaCode = playerInfo.AreaCode,
                            Balance = playerInfo.Balance,
                            Phone = playerInfo.Phone,
                            PlayerCode = playerInfo.PlayerCode,
                            RegisteredTime = playerInfo.RegisteredTime,
                            RegisterHost = playerInfo.RegisterHost,
                            Status = playerInfo.Status,
                            Username = playerInfo.Username,
                            VipLevel = playerInfo.VipLevel
                        })
                        .ApplyOrderBy(paging)
                        .ToArrayAsync()
                        .ConfigureAwait(false)
                };
            }

            paging.Page = pagingRequest.Page;
            paging.RowCount = pagingRequest.Rows;

            return new PagedResponse<PlayerInfoModel>
            {
                TotalCount = await playerInfos.CountAsync().ConfigureAwait(false),
                Items = await playerInfos
                    .Select(playerInfo => new PlayerInfoModel
                    {
                        AreaCode = playerInfo.AreaCode,
                        Balance = playerInfo.Balance,
                        Phone = playerInfo.Phone,
                        PlayerCode = playerInfo.PlayerCode,
                        RegisteredTime = playerInfo.RegisteredTime,
                        RegisterHost = playerInfo.RegisterHost,
                        Status = playerInfo.Status,
                        Username = playerInfo.Username,
                        VipLevel = playerInfo.VipLevel
                    })
                    .ApplyOrderBy(paging)
                    .ApplyPaging(paging)
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpGet("list/viplevels")]
        [Description("Get player vip levels")]
        [TgsResponseWrapper]
        public IAsyncEnumerable<VipLevelInfoModel> GetPlayerVipLevelsAsync(
            [FromServices] IQueryService<GetPlayerVipLevelsQuery, VipLevelInfo> queryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new GetPlayerVipLevelsQuery(brandId);

            return queryService.QueryAsync(query)
                .Select(info => new VipLevelInfoModel
                {
                    Id = info.Id,
                    Level = info.Level
                });
        }
    }
}
