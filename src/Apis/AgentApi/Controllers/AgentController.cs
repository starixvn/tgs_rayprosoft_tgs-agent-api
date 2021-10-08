using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Agent;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Agent;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentController : ControllerBase
    {
        private readonly IReportClient _client;
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator, IReportClient client)
        {
            _mediator = mediator;
            _client = client;
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
                DividendPercentage = request.DividendPercentage,
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

            return new CreateAgentResponse
            {
                IsNew = info.IsNew,
                AgentId = info.AgentId,
                Message = info.Message
            };
        }

        /// <summary>
        /// Get account changes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        [HttpGet("accountChanges")]
        [Description("Get account changes")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentAccountChangeModel>> GetAccountChangesAsync(
                   [FromQuery] GetAccountChangesRequest request,
                   [FromQuery] PagingRequest pagingRequest)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var accountChangesRequest = new GetAgentAccountChangesRequest
            {
                AgentId = agentTokenData.AgentId,
                BrandId = brandId,
                From = request.From,
                To = request.To,
                TransactionType = request.TransactionType,
                Page = pagingRequest.Page,
                Rows = pagingRequest.Rows,
                OrderBy = pagingRequest.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = pagingRequest.SortBy,
                ShowAll = pagingRequest.ShowAll
            };

            var response = await _client.GetAgentAccountChangesAsync(accountChangesRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentAccountChangeModel>()
            };
        }

        /// <summary>
        /// Get account changes transaction types
        /// </summary>
        /// <returns></returns>
        [HttpGet("accountChanges/TransactionTypes")]
        [Description("Get account changes transaction types")]
        public Task<Dictionary<int, string>> GetAccountChangesTransactionTypesAsync()
        {
            var allowType = new TransactionType[]
           {
                TransactionType.Deposit,
                TransactionType.Withdrawal,
                TransactionType.CancelWithdrawal,
                TransactionType.Adjust,
                TransactionType.Dividend
           };

            var dict = Enum.GetValues(typeof(TransactionType))
                   .Cast<TransactionType>()
                   .Where(x => allowType.Contains(x))
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
        /// Agent login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);
    }
}
