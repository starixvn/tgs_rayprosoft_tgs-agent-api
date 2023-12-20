using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.CashService.Client;
using Tgs.Report.Client;
using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.Agent;
using Tgs.Report.Client.Models.Round;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Controllers;

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
    public async Task<Models.PagingResponse<AgentAccountChangeModel>> GetAccountChangesAsync(
        [FromQuery] GetAccountChangesRequest request,
        [FromQuery] PagingRequest pagingRequest,
        [FromServices] ICashServiceClient cashServiceClient,
        [FromServices] IAgentClient agentClient)
    {
        var agentData = HttpContext.Items.GetAgentTokenData();

        var queryTransactionTypes = request.TransactionType is null
            ? Enum.GetValues(typeof(Models.Enums.TransactionType))
                .Cast<Models.Enums.TransactionType>()
                .Where(x => x is not Models.Enums.TransactionType.WithdrawCommit
                    and not Models.Enums.TransactionType.TransferCommit
                    and not Models.Enums.TransactionType.Receive)
                .Select(x => (int)x)
                .ToArray()
            : request.TransactionType.Value switch
            {
                TransactionType.Deposit => new[]
                {
                    (int)Models.Enums.TransactionType.Deposit, 
                    (int)Models.Enums.TransactionType.Deposited
                },
                TransactionType.Withdrawal => new[]
                {
                    (int)Models.Enums.TransactionType.Withdrawal, 
                    (int)Models.Enums.TransactionType.WithdrawLock
                },
                TransactionType.CancelWithdrawal => new[]
                {
                    (int)Models.Enums.TransactionType.CancelWithdrawal,
                    (int)Models.Enums.TransactionType.WithdrawRollback
                },
                TransactionType.Dividend => new[]
                {
                    (int)Models.Enums.TransactionType.AgentDividend
                },
                _ => new[] { (int)request.TransactionType.Value },
            };

        var agent = await agentClient.GetAgentInfoAsync(new GetAgentInfoRequest(agentData.AgentId));

        var response = await cashServiceClient.QueryTransactionLogsAsync(agent!.WalletId!.Value,
            request.From.UtcDateTime,
            request.To.UtcDateTime,
            queryTransactionTypes,
            new PagingConditions(pagingRequest.Page, pagingRequest.Rows));

        return new Models.PagingResponse<AgentAccountChangeModel>
        {
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new AgentAccountChangeModel
            {
                TransactionId = x.Id,
                Amount = x.Amount,
                Balance = x.Balance,
                TransactionType = (short)x.TransactionType,
                RecordedOn = x.RecordedOn
            })
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
            TransactionType.CompanyAdjust,
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
    public async Task<Models.PagingResponse<AgentDividendDetailModel>>
        GetAgentDividendDetailsAsync([FromQuery] GetDividendDetailsRequest request)
    {
        var dividendRequest = new GetAgentDividendDetailsRequest
        {
            AgentId = request.AgentId,
            AgentDividendPercentage = request.AgentDividendPercentage,
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
    public async Task<Models.PagingResponse<AgentDividendTeamDetailModel>>
        GetAgentDividendDetailsAsync([FromQuery] GetDividendTeamDetailsRequest request)
    {
        var dividendRequest = new GetAgentDividendTeamDetailsRequest
        {
            AgentId = request.AgentId,
            AgentDividendPercentage = request.AgentDividendPercentage,
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

        var key =
            $"agentapi:contributions:{brandId}:{agentTokenData.AgentId}:{request.From.ToUnixTimeMilliseconds()}:{request.To.ToUnixTimeMilliseconds()}";

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
            x.DepositHandlingFee,
            x.WithdrawalAmount,
            x.CompanyAdjustAmount,
            x.AgentAdjustAmount,
            x.BonusAdjustAmount,
            x.BonusAmount,
            x.BetAmount,
            x.ValidBetAmount,
            x.Turnover,
            x.WinLossAmount,
            x.NetProfit)).Skip(page * pageSize).Take(pageSize).ToArray();

        var total = new AgentContributionTotal(
            response.Items.Sum(x => x.DepositAmount),
            response.Items.Sum(x => x.DepositHandlingFee),
            response.Items.Sum(x => x.WithdrawalAmount),
            response.Items.Sum(x => x.CompanyAdjustAmount),
            response.Items.Sum(x => x.BonusAdjustAmount),
            response.Items.Sum(x => x.AgentAdjustAmount),
            response.Items.Sum(x => x.BonusAmount),
            response.Items.Sum(x => x.BetAmount),
            response.Items.Sum(x => x.ValidBetAmount),
            response.Items.Sum(x => x.Turnover),
            response.Items.Sum(x => x.WinLossAmount),
            response.Items.Sum(x => x.NetProfit));

        return new()
        {
            TotalCount = response.TotalCount,
            Items = items ?? Array.Empty<AgentContribution>(),
            Total = total
        };
    }

    /// <summary>
    ///     Get agent dividends
    /// </summary>
    [HttpGet("dividends")]
    [Description("Get dividends")]
    [Consumes(MediaTypeNames.Application.Json)]
    [TgsResponseWrapper]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<AgentDividendResponse> GetDividendsAsync(
        [FromQuery] GetDividendsRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();
        var brandId = HttpContext.Items.GetBrandId();

        var key =
            $"agentapi:dividends:{brandId}:{agentTokenData.AgentId}:{request.From.ToUnixTimeMilliseconds()}:{request.To.ToUnixTimeMilliseconds()}";
        if (!_memoryCache.TryGetValue(key, out PagedResponse<AgentDividendModel> response))
        {
            var dividendRequest = new GetAgentDividendsRequest
            {
                BrandId = brandId,
                AgentId = agentTokenData.AgentId,
                From = request.From,
                To = request.To,
                Page = 1,
                Rows = int.MaxValue,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = true
            };

            response = await _client.GetAgentDividendsAsync(dividendRequest);

            _memoryCache.Set(key, response, TimeSpan.FromMinutes(5));
        }

        var page = request.Page <= 0 ? 0 : request.Page - 1;
        var pageSize = request.Rows <= 0 ? 10 : request.Rows;
        var items = response.Items.Select(x => new AgentDividend(
            x.Id,
            x.Username,
            x.AgentAdjustAmount,
            x.BonusAdjustAmount,
            x.BonusAmount,
            x.BetAmount,
            x.ValidBetAmount,
            x.Turnover,
            x.WinLossAmount,
            x.NetProfit,
            x.AgentDividendAmount,
            x.FromPlayerAgentDividendAmount,
            x.FromTeamAgentDividendAmount,
            x.AgentDividendPercentage,
            x.AgentCoveredBonus,
            x.AgentCoveredBonusAdjust)).Skip(page * pageSize).Take(pageSize).ToArray();

        var total = new AgentDividendTotal(
            response.Items.Sum(x => x.BonusAdjustAmount),
            response.Items.Sum(x => x.AgentAdjustAmount),
            response.Items.Sum(x => x.BonusAmount),
            response.Items.Sum(x => x.BetAmount),
            response.Items.Sum(x => x.ValidBetAmount),
            response.Items.Sum(x => x.Turnover),
            response.Items.Sum(x => x.WinLossAmount),
            response.Items.Sum(x => x.NetProfit),
            response.Items.Sum(x => x.AgentDividendAmount),
            response.Items.Sum(x => x.FromPlayerAgentDividendAmount),
            response.Items.Sum(x => x.FromTeamAgentDividendAmount),
            response.Items.Sum(x => x.AgentCoveredBonus) ?? 0M,
            response.Items.Sum(x => x.AgentCoveredBonusAdjust) ?? 0M);

        return new()
        {
            TotalCount = response.TotalCount,
            Items = items,
            Total = total
        };
    }

    /// <summary>
    ///     Get agent dividends
    /// </summary>
    [HttpGet("agentDividends")]
    [Description("Get dividends")]
    [Consumes(MediaTypeNames.Application.Json)]
    [TgsResponseWrapper]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<MainSubAgentDividendResponse> GetAgentDividendsAsync(
        [FromQuery] GetDividendsRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();
        var brandId = HttpContext.Items.GetBrandId();

        var key =
            $"agentapi:agentdividends:{brandId}:{agentTokenData.AgentId}:{request.From.ToUnixTimeMilliseconds()}:{request.To.ToUnixTimeMilliseconds()}";
        if (!_memoryCache.TryGetValue(key,
                out (PagedResponse<AgentDividendModel> Main, PagedResponse<AgentDividendModel> Sub) response))
        {
            var singleDividendRequest = new GetSingleAgentDividendsRequest
            {
                BrandId = brandId,
                AgentId = agentTokenData.AgentId,
                From = request.From,
                To = request.To,
                Page = 1,
                Rows = int.MaxValue,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = true
            };

            response.Main = await _client.GetSingleAgentDividendsAsync(singleDividendRequest);

            var subDividendRequest = new GetSubAgentDividendsRequest
            {
                BrandId = brandId,
                AgentId = agentTokenData.AgentId,
                From = request.From,
                To = request.To,
                Page = 1,
                Rows = int.MaxValue,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = true
            };

            response.Sub = await _client.GetSubAgentDividendsAsync(subDividendRequest);

            _memoryCache.Set(key, response, TimeSpan.FromMinutes(5));
        }

        var page = request.Page <= 0 ? 0 : request.Page - 1;
        var pageSize = request.Rows <= 0 ? 10 : request.Rows;

        var mainItems = response.Main.Items.Select(x => new AgentDividend(
            x.Id,
            x.Username,
            x.AgentAdjustAmount,
            x.BonusAdjustAmount,
            x.BonusAmount,
            x.BetAmount,
            x.ValidBetAmount,
            x.Turnover,
            x.WinLossAmount,
            x.NetProfit,
            x.AgentDividendAmount,
            x.FromPlayerAgentDividendAmount,
            x.FromTeamAgentDividendAmount,
            x.AgentDividendPercentage,
            x.AgentCoveredBonus,
            x.AgentCoveredBonusAdjust)).ToArray();

        var subItems = response.Sub.Items.Select(x => new AgentDividend(
            x.Id,
            x.Username,
            x.AgentAdjustAmount,
            x.BonusAdjustAmount,
            x.BonusAmount,
            x.BetAmount,
            x.ValidBetAmount,
            x.Turnover,
            x.WinLossAmount,
            x.NetProfit,
            x.AgentDividendAmount,
            x.FromPlayerAgentDividendAmount,
            x.FromTeamAgentDividendAmount,
            x.AgentDividendPercentage,
            x.AgentCoveredBonus,
            x.AgentCoveredBonusAdjust)).Skip(page * pageSize).Take(pageSize).ToArray();

        var subTotal = new AgentDividendTotal(
            response.Sub.Items.Sum(x => x.BonusAdjustAmount),
            response.Sub.Items.Sum(x => x.AgentAdjustAmount),
            response.Sub.Items.Sum(x => x.BonusAmount),
            response.Sub.Items.Sum(x => x.BetAmount),
            response.Sub.Items.Sum(x => x.ValidBetAmount),
            response.Sub.Items.Sum(x => x.Turnover),
            response.Sub.Items.Sum(x => x.WinLossAmount),
            response.Sub.Items.Sum(x => x.NetProfit),
            response.Sub.Items.Sum(x => x.AgentDividendAmount),
            response.Sub.Items.Sum(x => x.FromPlayerAgentDividendAmount),
            response.Sub.Items.Sum(x => x.FromTeamAgentDividendAmount),
            response.Sub.Items.Sum(x => x.AgentCoveredBonus) ?? 0M,
            response.Sub.Items.Sum(x => x.AgentCoveredBonusAdjust) ?? 0M);

        return new MainSubAgentDividendResponse
        {
            SubTotalCount = response.Sub.TotalCount,
            MainItems = mainItems,
            SubItems = subItems,
            SubTotal = subTotal
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
    public async Task<Models.PagingResponse<RoundModel>> GetRoundsAsync(
        [FromQuery] PagingRequest pagingRequest,
        [FromQuery] GetAgentPlayerRoundsReportRequest request)
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
}
