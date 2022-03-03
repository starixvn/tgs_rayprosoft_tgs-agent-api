using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tgs.Report.Client;
using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.Agent;
using Tgs.Report.Client.Models.Round;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentReportController : ControllerBase
    {
        private readonly IReportClient _client;
        private readonly IMemoryCache _memoryCache;

        public AgentReportController(
            IReportClient client,
            IMemoryCache memoryCache
            )
        {
            _client = client;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// Get account changes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        [HttpGet("accountchanges")]
        [Description("Get account changes")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentAccountChangeModel>> GetAccountChangesAsync(
            [FromQuery] GetAccountChangesRequest request,
            [FromQuery] PagingRequest pagingRequest,
            [FromQuery] Guid id)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var accountChangesRequest = new GetAgentAccountChangesRequest
            {
                AgentId = id,
                BrandId = brandId,
                From = request.From,
                To = request.To,
                TransactionType = (short?)request.TransactionType,
                Page = pagingRequest.Page,
                Rows = pagingRequest.Rows,
                OrderBy = pagingRequest.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
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
        [HttpGet("accountchanges/transactiontypes")]
        [Description("Get account changes transaction types")]
        public Task<Dictionary<int, string>> GetAccountChangesTransactionTypesAsync()
        {
            var allowType = new[]
            {
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

        [HttpGet("dividendDetails")]
        [Description("Get agent dividend details")]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentDividendDetailModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetDividendDetailsRequest request)
        {

            var dividendRequest = new GetAgentDividendDetailsRequest
            {
                AgentId = request.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };
            var response = await _client.GetAgentDividendDetailsAsync(dividendRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentDividendDetailModel>()
            };
        }

        [HttpGet("dividendTeamDetails")]
        [Description("Get agent dividend team details")]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentDividendTeamDetailModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetDividendTeamDetailsRequest request)
        {
            var dividendRequest = new GetAgentDividendTeamDetailsRequest
            {
                AgentId = request.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };
            var response = await _client.GetAgentDividendTeamDetailsAsync(dividendRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentDividendTeamDetailModel>()
            };
        }

        /// <summary>
        /// Get rounds query status
        /// </summary>
        /// <returns></returns>
        [HttpGet("rounds/status")]
        [Description("Get rounds query status")]
        public Task<Dictionary<int, string>> GetClientRoundStatusAsync()
        {
            var dict = Enum.GetValues(typeof(ClientRoundStatus))
                .Cast<ClientRoundStatus>()
                .ToDictionary(t => (int)t, t => t.ToString());

            return Task.FromResult(dict);
        }

        /// <summary>
        ///     Get agent contributions
        /// </summary>
        [HttpGet("contributions")]
        [Description("Get contributions")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = false)]
        public async Task<AgentContributionResponse> GetContributionsAsync(
            [FromQuery] GetContributionsRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var key = $"contributions:{brandId}:{agentTokenData.AgentId}:{request.From.ToUnixTimeMilliseconds()}:{request.To.ToUnixTimeMilliseconds()}";
            if (!_memoryCache.TryGetValue(key, out PagedResponse<AgentContributionModel> response))
            {
                var contributionRequest = new GetAgentContributionsRequest
                {
                    BrandId = brandId,
                    AgentId = agentTokenData.AgentId,
                    IncludeAgents = true,
                    From = request.From,
                    To = request.To,
                    Page = 1,
                    Rows = int.MaxValue,
                    OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                    SortBy = request.SortBy,
                    ShowAll = true
                };

                response = await _client.GetAgentContributionsAsync(contributionRequest);

                if (response != null)
                {
                    _memoryCache.Set(key, response, TimeSpan.FromMinutes(5));
                }
            }

            var page = request.Page <= 0 ? 0 : request.Page - 1;
            var pageSize = request.Rows <= 0 ? 10 : request.Rows;
            var items = response.Items.Select(x => new AgentContribution(
                x.Username,
                x.Type,
                x.DepositAmount,
                x.WithdrawalAmount,
                x.AdjustmentAmount,
                x.BonusAmount,
                x.BetAmount,
                x.ValidBetAmount,
                x.WinLossAmount,
                x.NetProfit)).Skip(page * pageSize).Take(pageSize).ToArray();

            var total = new AgentContributionTotal(
                    DepositAmount: response.Items.Sum(x => x.DepositAmount),
                    WithdrawalAmount: response.Items.Sum(x => x.WithdrawalAmount),
                    AdjustmentAmount: response.Items.Sum(x => x.AdjustmentAmount),
                    BonusAmount: response.Items.Sum(x => x.BonusAmount),
                    BetAmount: response.Items.Sum(x => x.BetAmount),
                    ValidBetAmount: response.Items.Sum(x => x.ValidBetAmount),
                    Turnover: response.Items.Sum(x => x.Turnover),
                    WinLossAmount: response.Items.Sum(x => x.WinLossAmount),
                    NetProfit: response.Items.Sum(x => x.NetProfit));


            return new(
                TotalCount: response.TotalCount,
                Items: items ?? Array.Empty<AgentContribution>(),
                Total: total);
        }

        /// <summary>
        ///     Get agent dividends
        /// </summary>
        [HttpGet("dividends")]
        [Description("Get dividends")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentDividendModel>> GetDividendsAsync(
            [FromQuery] GetDividendsRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var dividendRequest = new GetAgentDividendsRequest
            {
                BrandId = brandId,
                AgentId = agentTokenData.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentDividendsAsync(dividendRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentDividendModel>()
            };
        }

        /// <summary>
        /// Get player rounds
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("rounds")]
        [Description("Get player rounds")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<RoundModel>> GetRoundsAsync(
           [FromQuery] PagingRequest pagingRequest,
           [FromQuery] GetRoundsReportRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var roundsRequest = new GetAgentPlayerRoundsRequest
            {
                AgentId = agentTokenData.AgentId,
                BrandId = brandId,
                Username = request.Username,
                RoundId = request.RoundId,
                ExternalRoundId = request.ExternalRoundId,
                GameProviderId = request.GameProviderId,
                Status = (short?)request.Status,
                From = request.From,
                To = request.To,
                ClosedFrom = request.ClosedFrom,
                ClosedTo = request.ClosedTo,
                Page = pagingRequest.Page,
                Rows = pagingRequest.Rows,
                OrderBy = pagingRequest.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = pagingRequest.SortBy,
                ShowAll = pagingRequest.ShowAll
            };

            var response = await _client.GetAgentPlayerRoundsAsync(roundsRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<RoundModel>()
            };
        }

        /// <summary>
        /// Get team Information
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        [HttpGet("teaminfo")]
        [Description("Get team Information")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<TeamInfoModel>> GetTeamInformationAsync(
            [FromQuery] PagingRequest pagingRequest,
            [FromQuery] Guid id)
        {
            var request = new GetAgentTeamInfoRequest
            {
                AgentId = id,
                Page = pagingRequest.Page,
                Rows = pagingRequest.Rows,
                OrderBy = pagingRequest.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = pagingRequest.SortBy,
                ShowAll = pagingRequest.ShowAll
            };

            var response = await _client.GetAgentTeamInfoAsync(request);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<TeamInfoModel>()
            };
        }
    }
}
