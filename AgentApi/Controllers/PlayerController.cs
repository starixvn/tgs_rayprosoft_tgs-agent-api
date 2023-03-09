using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.Paging;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Player;
using TGS.Services.Agent.Exceptions;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    [HttpPost("check/registrationConditions", Name = "CheckRegistrationConditions")]
    [Description("Check registration conditions")]
    [TgsResponseWrapper]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask<bool> CheckRegistrationConditionsAsync(
        [FromServices] IGetService<CheckRegistrationQuery, bool> queryService,
        [FromBody] CheckRegistrationRequest request
        )
    {
        var brandId = HttpContext.Items.GetBrandId();

        var query = new CheckRegistrationQuery(
            brandId,
            request.Username,
            request.Data);

        return queryService.GetAsync(query);
    }

    [HttpPost("create", Name = "CreatePlayer")]
    [Description("Create a player")]
    [TgsResponseWrapper]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask<string> CreatePlayerAsync(
        [FromServices] IGetService<CreatePlayerQuery, string> queryService,
        [FromBody] CreatePlayerRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var agentTokenData = HttpContext.Items.GetAgentTokenData();
        var ipAddress = HttpContext.Connection.RemoteIpAddress;
        var hostName = Request.Host.Host;

        var query = new CreatePlayerQuery(
            brandId,
            agentTokenData.AgentId,
            request.Username,
            request.Password,
            ipAddress!.ToString(),
            hostName);

        return queryService.GetAsync(query);
    }

    [HttpGet("balance/{playerCode}",Name = "GetPlayerBalance")]
    [Description("Get a player balance")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask<decimal> GetPlayerBalanceAsync(
        [FromServices] IGetService<GetPlayerBalanceQuery, decimal> getService,
        string playerCode)
    {
        var agentToken = HttpContext.Items.GetAgentTokenData();

        return getService.GetAsync(new GetPlayerBalanceQuery(agentToken.AgentId, playerCode));
    }

    [HttpGet("list", Name = "GetPlayerInfos")]
    [Description("Get players of agent")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask<PagingResponse<PlayerInfoModel>> GetPlayerInfosAsync(
        [FromServices] IQueryService<GetAgentPlayerInfosQuery, AgentPlayerInfo> queryService,
        [FromServices] IGetService<GetAgentPlayerInfosQuery, int> countService,
        [FromQuery] GetAgentPlayersRequest request,
        [FromQuery] PagingRequest pagingRequest)
    {
        var agentToken = HttpContext.Items.GetAgentTokenData();

        var paging = new Paging<PlayerInfoModel>();

        if (pagingRequest.ShowAll)
        {
            throw new InvalidOperationException("showall is not supported");
        }

        var query = new GetAgentPlayerInfosQuery(
            agentToken.AgentId,
            request.Username,
            request.BeginRegisteredOn,
            request.EndRegisteredOn,
            request.RegisterHost,
            request.Status,
            request.VipId,
            new PagingInfo(pagingRequest.Page, pagingRequest.Rows, pagingRequest.SortBy, paging.IsDesc));

        return new PagingResponse<PlayerInfoModel>
        {
            TotalCount = await countService.GetAsync(query).ConfigureAwait(false),
            Items = queryService.QueryAsync(query)
                .Select(playerInfo => new PlayerInfoModel
                {
                    AreaCode = playerInfo.AreaCode,
                    Phone = playerInfo.Phone,
                    PlayerCode = playerInfo.PlayerCode,
                    RegisteredTime = playerInfo.RegisteredTime,
                    RegisterHost = playerInfo.RegisterHost,
                    Status = playerInfo.Status,
                    Username = playerInfo.Username,
                    VipLevel = playerInfo.VipLevel
                })
                .ToEnumerable()
                
        };
    }

    [HttpGet("list/statuses", Name = "GetPlayerStatusTypes")]
    [Description("Get player status types")]
    [TgsResponseWrapper]
    public ValueTask<Dictionary<int, string>> GetPlayerStatusTypesAsync()
    {
        var dict = Enum.GetValues(typeof(PlayerStatus))
            .Cast<PlayerStatus>()
            .ToDictionary(t => (int)t, t => t.ToString());

        return ValueTask.FromResult(dict);
    }

    [HttpGet("list/viplevels", Name = "GetVipLevels")]
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

    [HttpGet("withdraw/amount/{playerCode}",Name = "GetPlayerWithdrawRequireAmount")]
    [Description("Get a player withdraw require amount")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<decimal> GetPlayerWithdrawRequireAmountAsync(
        [FromServices] IGetService<GetPlayerWithdrawRequireAmountQuery, decimal> getService,
        string playerCode)
    {
        var query = new GetPlayerWithdrawRequireAmountQuery(playerCode);

        return await getService.GetAsync(query).ConfigureAwait(false);
    }

    [HttpPut("email", Name = "SetEmail")]
    [Description("Set player email")]
    [TgsResponseWrapper]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask SetPlayerEmailAsync(
        [FromServices] ICommandService<SetPlayerEmailCommand> commandService,
        [FromBody] SetPlayerEmailRequest request)
    {
        var command = new SetPlayerEmailCommand(request.PlayerCode, request.Email);

        return commandService.ExecuteAsync(command);
    }

    [HttpPut("phone", Name = "SetPhone")]
    [Description("Set player phone")]
    [TgsResponseWrapper]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask SetPlayerPhoneAsync(
        [FromServices] ICommandService<SetPlayerPhoneCommand> commandService,
        [FromBody] SetPlayerPhoneRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone)
            || string.IsNullOrWhiteSpace(request.AreaCode))
            throw new PhoneRequiredException();

        var command = new SetPlayerPhoneCommand(request.PlayerCode, request.AreaCode, request.Phone);

        return commandService.ExecuteAsync(command);
    }
}
