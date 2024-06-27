using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Paging;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices;
using TGS.Services.Agent.ApplicationServices.Agent;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Operations.Validators;
using Tgs.ServiceUtility;
using AgentNotFoundException = TGS.Core.Models.Exceptions.AgentNotFoundException;
using CreateAgentRequest = TGS.Apis.AgentApi.Models.CreateAgentRequest;
using Tgs.PlayerService.Client;
using Tgs.CashService.Client;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class AgentController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Check username existed
    /// </summary>
    [HttpGet("checkUsername")]
    public async Task<CheckUsernameResponse> CheckUsernameAsync(
        [FromServices] IGetService<CheckAgentExistsQuery, bool> getUserNameQueryService,
        [FromQuery] CheckUsernameRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();

        var query = new CheckAgentExistsQuery(brandId, request.Username);
        var hasExisted = await getUserNameQueryService.GetAsync(query).ConfigureAwait(false);

        return new CheckUsernameResponse { Exists = hasExisted };
    }

    [HttpPost("create")]
    [CheckAgentEnabled]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<CreateAgentResponse> CreateAgentAsync(
        [FromServices] IGetService<CreateAgentCommand, CreateAgentInfo> getCreateAgentQueryService,
        [FromBody] CreateAgentRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var query = new CreateAgentCommand
        {
            BrandId = brandId,
            DividendPercentage = request.DividendPercentage,
            CardGameRebatePercentage = request.CardGameRebatePercentage,
            LiveDealerRebatePercentage = request.LiveDealerRebatePercentage,
            LotteryRebatePercentage = request.LotteryRebatePercentage,
            ParentAgentId = agentTokenData.AgentId,
            Password = request.Password,
            RealName = request.RealName,
            Email = request.Email,
            Phone = request.Phone,
            Remarks = request.Remarks,
            SlotRebatePercentage = request.SlotRebatePercentage,
            SportsRebatePercentage = request.SportsRebatePercentage,
            Username = request.UserName,
            VirtualSportsRebatePercentage = request.VirtualSportsRebatePercentage
        };

        var info = await getCreateAgentQueryService
            .GetAsync(query)
            .ConfigureAwait(false);

        return new CreateAgentResponse { IsNew = info.IsNew, AgentId = info.AgentId, Message = info.Message };
    }

    [HttpGet("list/statuses")]
    [Description("Get agent list status types")]
    public Task<Dictionary<int, string>> GetAccountChangesTransactionTypesAsync()
    {
        var dict = Enum.GetValues(typeof(AgentStatus))
            .Cast<AgentStatus>()
            .ToDictionary(t => (int)t, t => t.ToString());

        return Task.FromResult(dict);
    }

    /// <summary>
    /// Get an agent information by id
    /// </summary>
    [HttpGet("info/{agentId}")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<GetAgentResponse> GetAgentInfoByIdAsync(
        [FromServices] IStatusValidator statusValidator,
        [FromServices] IGetService<GetAgentInfoQuery, AgentInfo> getAgentService,
        [FromServices] IGetService<GetBrandInfoQuery, BrandInfo> getBrandService,
        [FromServices] IQueryService<GetAgentDividendConditionsByIdQuery, DividendCondition> queryDividendsService,
        Guid agentId)
    {
        var brandId = HttpContext.Items.GetBrandId();
        await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

        var brand = await getBrandService
            .GetAsync(new GetBrandInfoQuery(brandId))
            .ConfigureAwait(false) ?? throw new BrandNotFoundException();

        var agentInfo = await getAgentService
                            .GetAsync(new GetAgentInfoQuery(agentId))
                            .ConfigureAwait(false)
                        ?? throw new AgentNotFoundException(agentId.ToString(), Resources.Agent.NotFound);

        var parentAgentInfo = agentInfo.ParentAgentId == null
            ? null
            : await getAgentService
                .GetAsync(new GetAgentInfoQuery(agentInfo.ParentAgentId.Value))
                .ConfigureAwait(false);

        return new GetAgentResponse
        {
            Data = new AgentData
            {
                Id = agentInfo.Id,
                Username = agentInfo.Username,
                RealName = agentInfo.RealName,
                Email = agentInfo.Email,
                Phone = agentInfo.Phone,
                QQ = agentInfo.QQ,
                Wechat = agentInfo.Wechat,
                InvitationCode = agentInfo.InvitationCode,
                IsSetPayPassword = agentInfo.PayPassword.Length > 0,
                AgentLevel = (int)agentInfo.AgentLevelType,
                DividendPercentage = agentInfo.DividendPercentage,
                ParentUsername = parentAgentInfo?.Username,
                RegisteredOn = agentInfo.RegisteredOn,
                Status = agentInfo.Status.ToString(),
                Type = agentInfo.Type.ToString(),
                LevelType = agentInfo.AgentLevelType.ToString(),
                AgentDividendType = brand.AgentDividendType.ToString(),
                AccumulatedProfitSharing = agentInfo.AccumulatedProfitSharing,
                DividendConditions = await queryDividendsService
                    .QueryAsync(new GetAgentDividendConditionsByIdQuery(brandId, agentId))
                    .OrderBy(dc => dc.Rank)
                    .Select(x => new DividendConditionModel
                    {
                        Rank = x.Rank,
                        NetProfit = x.NetProfit,
                        ActiveNumber = x.ActiveNumber,
                        DividendPercentage = x.DividendPercentage
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            }
        };
    }

    [HttpGet("list")]
    [Description("Get agents")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<PagedResponse<AgentModel>> GetAgentsAsync(
        [FromServices] IQueryService<GetAgentInfosQuery, AgentInfos> queryService,
        [FromServices] IAgentClient agentClient,
        [FromServices] IPlayerServiceClient playerServiceClient,
        [FromServices] ICashServiceClient cashServiceClient,
        [FromQuery] GetAgentsRequest request,
        [FromQuery] PagingRequest pagingRequest)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var query = new GetAgentInfosQuery(
            brandId,
            agentTokenData.AgentId,
            request.Status,
            request.Username);

        var agentInfos = queryService.QueryAsync(query);

        var paging = new Paging<AgentModel>();

        if (!string.IsNullOrWhiteSpace(pagingRequest.SortBy))
            paging.SortByPropertyName(pagingRequest.SortBy, pagingRequest.IsDesc);

        var childAgentInfos = await agentClient
            .GetChildAgentInfosAsync(new GetChildAgentInfosRequest(agentTokenData.AgentId), HttpContext.RequestAborted)
            .ToArrayAsync(HttpContext.RequestAborted);

        var agentPlayerCounts =
            await playerServiceClient
                .GetPlayerCountInfosByBrandIdAsync(brandId, false, HttpContext.RequestAborted)
                .ToArrayAsync(HttpContext.RequestAborted);

        if (pagingRequest.ShowAll)
            return new PagedResponse<AgentModel>
            {
                TotalCount = await agentInfos.CountAsync().ConfigureAwait(false),
                Items = await agentInfos
                    .SelectAwait(async agentInfo =>
                    {
                        var agentIds = Array.Empty<Guid>();

                        GetAgentIds(agentInfo.AgentId, childAgentInfos, ref agentIds);

                        agentIds = [.. agentIds, agentInfo.AgentId];

                        var playerCount = agentPlayerCounts
                            .Where(a => a.AgentId is not null)
                            .Where(a => agentIds.Contains(a.AgentId.Value))
                        .Sum(s => s.PlayerCount);

                        var balance = await cashServiceClient.GetBalanceAsync(agentInfo.WalletId, HttpContext.RequestAborted);

                        return new AgentModel
                        {
                            DividendPercentage = agentInfo.DividendPercentage,
                            RegisteredOn = agentInfo.RegisteredOn,
                            Status = agentInfo.Status.ToString(),
                            TeamNumber = agentIds.Length + playerCount,
                            Type = agentInfo.Type.ToString(),
                            Username = agentInfo.Username,
                            AgnetId = agentInfo.AgentId,
                            Balance = balance
                        };
                    })
                    .ApplyOrderBy(paging)
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };

        paging.Page = pagingRequest.Page;
        paging.RowCount = pagingRequest.Rows;

        return new PagedResponse<AgentModel>
        {
            TotalCount = await agentInfos.CountAsync().ConfigureAwait(false),
            Items = await agentInfos
                .SelectAwait(async agentInfo =>
                {
                    var agentIds = Array.Empty<Guid>();

                    GetAgentIds(agentInfo.AgentId, childAgentInfos, ref agentIds);

                    agentIds = [.. agentIds, agentInfo.AgentId];

                    var playerCount = agentPlayerCounts
                        .Where(a => a.AgentId is not null)
                        .Where(a => agentIds.Contains(a.AgentId.Value))
                        .Sum(s => s.PlayerCount);

                    var balance = await cashServiceClient.GetBalanceAsync(agentInfo.WalletId, HttpContext.RequestAborted);

                    return new AgentModel
                    {
                        Balance = balance,
                        DividendPercentage = agentInfo.DividendPercentage,
                        RegisteredOn = agentInfo.RegisteredOn,
                        Status = agentInfo.Status.ToString(),
                        TeamNumber = agentIds.Length + playerCount,
                        Type = agentInfo.Type.ToString(),
                        Username = agentInfo.Username,
                        AgnetId = agentInfo.AgentId
                    };
                })
                .ApplyOrderBy(paging)
                .ApplyPaging(paging)
                .ToArrayAsync()
                .ConfigureAwait(false)
        };
    }

    /// <summary>
    /// Get an agent information
    /// </summary>
    [HttpGet("info")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<GetAgentResponse> GetLoginAgentInfoAsync(
        [FromServices] IStatusValidator statusValidator,
        [FromServices] IGetService<GetBrandInfoQuery, BrandInfo> getBrandService,
        [FromServices] IAgentClient agentClient,
        [FromServices] IGetService<GetAgentBalanceQuery, decimal> getAgentBalanceService,
        [FromServices] IQueryService<GetAgentDomainsQuery, string> getAgentDomainsService,
        [FromServices] IQueryService<GetAgentDividendConditionsByIdQuery, DividendCondition> queryDividendsService)
    {
        var brandId = HttpContext.Items.GetBrandId();
        await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

        var brand = await getBrandService
            .GetAsync(new GetBrandInfoQuery(brandId))
            .ConfigureAwait(false) ?? throw new BrandNotFoundException();

        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentId = agentTokenData.AgentId;

        var agentInfo = await agentClient
                            .GetAgentInfoAsync(new GetAgentInfoRequest(agentId), HttpContext.RequestAborted)
                            .ConfigureAwait(false)
                        ?? throw new AgentNotFoundException(agentId.ToString(), Resources.Agent.NotFound);

        var parentAgentInfo = agentInfo.ParentAgentId == null
            ? null
            : await agentClient
                .GetAgentInfoAsync(new GetAgentInfoRequest(agentInfo.ParentAgentId.Value), HttpContext.RequestAborted)
                .ConfigureAwait(false);

        return new GetAgentResponse
        {
            Data = new AgentData
            {
                Id = agentInfo.Id,
                Username = agentInfo.Username,
                RealName = agentInfo.RealName,
                Email = agentInfo.Email,
                Phone = agentInfo.Phone,
                QQ = agentInfo.QQ,
                Wechat = agentInfo.Wechat,
                InvitationCode = agentInfo.InvitationCode,
                BrandUrl = brand.DomainUrl,
                IsSetPayPassword = agentInfo.IsSetPayPassword,
                DividendPercentage = agentInfo.DividendPercentage,
                ParentUsername = parentAgentInfo?.Username,
                RegisteredOn = agentInfo.RegisteredOn,
                Balance = await getAgentBalanceService
                    .GetAsync(new GetAgentBalanceQuery(agentInfo.WalletId.Value, agentInfo.Id))
                    .ConfigureAwait(false),
                Status = agentInfo.Status.ToString(),
                Type = agentInfo.Type.ToString(),
                LevelType = agentInfo.AgentLevelType.ToString(),
                PlayerNumber = agentInfo.PlayerNumber,
                AgentDividendType = brand.AgentDividendType.ToString(),
                AccumulatedProfitSharing = agentInfo.AccumulatedProfitSharing,
                DividendConditions = await queryDividendsService
                    .QueryAsync(new GetAgentDividendConditionsByIdQuery(brandId, agentId))
                    .OrderBy(dc => dc.Rank)
                    .Select(x => new DividendConditionModel
                    {
                        Rank = x.Rank,
                        NetProfit = x.NetProfit,
                        ActiveNumber = x.ActiveNumber,
                        DividendPercentage = x.DividendPercentage
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false),
                AgentDomains = await getAgentDomainsService.QueryAsync(new GetAgentDomainsQuery(agentId))
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            }
        };
    }

    /// <summary>
    /// Agent login
    /// </summary>
    [HttpPost]
    [Route("login")]
    [Consumes(MediaTypeNames.Application.Json)]
    public Task<LoginResponse> LoginAsync(LoginRequest request) =>
        mediator.Send(request);

    private static void GetAgentIds(Guid id, ChildAgentInfoModel[] models, ref Guid[] agentIds)
    {
        var childAgents = models.Where(x => x.ParentAgentId == id).ToArray();

        if (childAgents.Length <= 0)
            return;

        {
            agentIds = [.. agentIds, .. childAgents.Select(x => x.AgentId)];

            foreach (var childAgentInfoModel in childAgents)
            {
                GetAgentIds(childAgentInfoModel.AgentId, models, ref agentIds);
            }
        }
    }
}