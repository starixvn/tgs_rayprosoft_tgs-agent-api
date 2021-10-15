using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Agent;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;
using AgentDividendConditionModel = TGS.Services.Agent.Services.Agent.AgentDividendConditionModel;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        [CheckAgentEnabled]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<CreateAgentResponse> CreateAgentAsync(
            [FromServices] IGetService<GetCreateAgentQuery, CreateAgentInfo> getCreateAgentQueryService,
            [FromBody] CreateAgentRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var query = new GetCreateAgentQuery
            {
                BrandId = brandId,
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
                VirtualSportsRebatePercentage = request.VirtualSportsRebatePercentage,
                AgentDividendConditions = request.AgentDividendConditions.Select(x => new AgentDividendConditionModel
                {
                    Rank = x.Rank,
                    NetProfit = x.NetProfit,
                    ActiveNumber = x.ActiveNumber,
                    DividendPercentage = x.DividendPercentage
                }).ToArray()
            };

            var info = await getCreateAgentQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            return new CreateAgentResponse
            {
                IsNew = info.IsNew,
                AgentId = info.AgentId,
                Message = info.Message
            };
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
        /// Get an agent information
        /// </summary>
        [HttpGet("info")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetAgentResponse> GetAgentInfoAsync(
            [FromServices] IStatusValidator statusValidator,
            [FromServices] IGetService<GetAgentInfoQuery, AgentInfo> getAgentService)
        {
            var brandId = HttpContext.Items.GetBrandId();
            await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var agentId = agentTokenData.AgentId;

            var agentInfo = await getAgentService.GetAsync(new GetAgentInfoQuery(agentId));

            if (agentInfo == null)
            {
                throw new AgentNotFoundException(agentId.ToString(), Resources.Agent.NotFound);
            }

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
                    BrandUrl = agentInfo.BrandUrl,
                    IsSetPayPassword = agentInfo.PayPassword.Length > 0,
                    AgentLevel = agentInfo.AgentLevel,
                    AccumulatedProfitSharing = agentInfo.AccumulatedProfitSharing,
                    DividendPercentage = agentInfo.DividendPercentage,
                    ParentUsername = agentInfo.ParentUserName,
                    RegisteredOn = agentInfo.RegisteredOn,
                    Balance = agentInfo.Balance,
                    Status = agentInfo.Status.ToString(),
                    Type = agentInfo.Type.ToString(),
                    LevelType = agentInfo.AgentLevelType.ToString(),
                    DividendConditions = agentInfo.AgentDividendConditions.Select(x => new DividendConditionModel
                    {
                        Rank = x.Rank,
                        NetProfit = x.NetProfit,
                        ActiveNumber = x.ActiveNumber,
                        DividendPercentage = x.DividendPercentage
                    }).OrderBy(x => x.Rank).ToArray()
                }
            };
        }

        /// <summary>
        /// Get an agent information by id
        /// </summary>
        [HttpGet("info/{agentId}")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetAgentResponse> GetAgentInfoByIdAsync(
            [FromServices] IStatusValidator statusValidator,
            [FromServices] IGetService<GetAgentInfoQuery, AgentInfo> getAgentService,
            Guid agentId)
        {
            var brandId = HttpContext.Items.GetBrandId();
            await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var agentInfo = await getAgentService.GetAsync(new GetAgentInfoQuery(agentId));

            if (agentInfo == null)
            {
                throw new AgentNotFoundException(agentId.ToString(), Resources.Agent.NotFound);
            }

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
                    BrandUrl = agentInfo.BrandUrl,
                    IsSetPayPassword = agentInfo.PayPassword.Length > 0,
                    AgentLevel = agentInfo.AgentLevel,
                    AccumulatedProfitSharing = agentInfo.AccumulatedProfitSharing,
                    DividendPercentage = agentInfo.DividendPercentage,
                    ParentUsername = agentInfo.ParentUserName,
                    RegisteredOn = agentInfo.RegisteredOn,
                    Balance = agentInfo.Balance,
                    Status = agentInfo.Status.ToString(),
                    Type = agentInfo.Type.ToString(),
                    LevelType = agentInfo.AgentLevelType.ToString(),
                    DividendConditions = agentInfo.AgentDividendConditions.Select(x => new DividendConditionModel
                    {
                        Rank = x.Rank,
                        NetProfit = x.NetProfit,
                        ActiveNumber = x.ActiveNumber,
                        DividendPercentage = x.DividendPercentage
                    }).OrderBy(x => x.Rank).ToArray()
                }
            };
        }

        [HttpGet("list")]
        [Description("Get agents")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagedResponse<AgentModel>> GetAgentsAsync(
            [FromServices] IQueryService<GetAgentInfosQuery, AgentInfos> queryService,
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
            {
                paging.SortByPropertyName(pagingRequest.SortBy, pagingRequest.IsDesc);
            }

            if (pagingRequest.ShowAll)
            {
                return new PagedResponse<AgentModel>
                {
                    TotalCount = await agentInfos.CountAsync().ConfigureAwait(false),
                    Items = await agentInfos
                        .Select(agentInfo => new AgentModel
                        {
                            AgentStatus = agentInfo.AgentStatus.ToString(),
                            Balance = agentInfo.Balance,
                            CardGameRebatePercentage = agentInfo.CardGameRebatePercentage,
                            CommissionPercentage = agentInfo.CommissionPercentage,
                            DividendPercentage = agentInfo.DividendPercentage,
                            LevelType = agentInfo.LevelType.ToString(),
                            LiveDealerRebatePercentage = agentInfo.LiveDealerRebatePercentage,
                            LotteryRebatePercentage = agentInfo.LotteryRebatePercentage,
                            RegisteredOn = agentInfo.RegisteredOn,
                            SlotRebatePercentage = agentInfo.SlotRebatePercentage,
                            SportsRebatePercentage = agentInfo.SportsRebatePercentage,
                            Status = agentInfo.Status.ToString(),
                            TeamNumber = agentInfo.TeamNumber,
                            Type = agentInfo.Type.ToString(),
                            Username = agentInfo.Username,
                            VirtualSportsRebatePercentage = agentInfo.VirtualSportsRebatePercentage,
                            AgnetId = agentInfo.AgentId
                        })
                        .ApplyOrderBy(paging)
                        .ToArrayAsync()
                        .ConfigureAwait(false)
                };
            }

            paging.Page = pagingRequest.Page;
            paging.RowCount = pagingRequest.Rows;

            return new PagedResponse<AgentModel>()
            {
                TotalCount = await agentInfos.CountAsync().ConfigureAwait(false),
                Items = await agentInfos
                    .Select(agentInfo => new AgentModel
                    {
                        AgentStatus = agentInfo.AgentStatus.ToString(),
                        Balance = agentInfo.Balance,
                        CardGameRebatePercentage = agentInfo.CardGameRebatePercentage,
                        CommissionPercentage = agentInfo.CommissionPercentage,
                        DividendPercentage = agentInfo.DividendPercentage,
                        LevelType = agentInfo.LevelType.ToString(),
                        LiveDealerRebatePercentage = agentInfo.LiveDealerRebatePercentage,
                        LotteryRebatePercentage = agentInfo.LotteryRebatePercentage,
                        RegisteredOn = agentInfo.RegisteredOn,
                        SlotRebatePercentage = agentInfo.SlotRebatePercentage,
                        SportsRebatePercentage = agentInfo.SportsRebatePercentage,
                        Status = agentInfo.Status.ToString(),
                        TeamNumber = agentInfo.TeamNumber,
                        Type = agentInfo.Type.ToString(),
                        Username = agentInfo.Username,
                        VirtualSportsRebatePercentage = agentInfo.VirtualSportsRebatePercentage,
                        AgnetId = agentInfo.AgentId
                    })
                    .ApplyOrderBy(paging)
                    .ApplyPaging(paging)
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        /// <summary>
        /// Agent login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);
    }
}
