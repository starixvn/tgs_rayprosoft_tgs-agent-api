using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Agent;
using TGS.Services.BackOffice.Application.Models.ReportAgent;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public AgentReportController(IReportClient client) => _client = client;

        [HttpGet("contributions")]
        [Description("Get agent contributions")]
        public async Task<GetAgentContributionsReportResponse> GetRoundsAsync(
            [FromServices] IGetService<GetAgentContributionsReportRequest, GetAgentContributionsReportResponse>
                getAgentContributionsReportService,
            [FromQuery] GetAgentContributionsReportRequest request)
            => await getAgentContributionsReportService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("{id}/teamInfo")]
        [Description("Get agent team Information")]
        public async Task<PagedResponse<TeamInfoReportModel>> GetAgentTeamInformationAsync(
            Guid id,
            [FromQuery] GetAgentTeamInfoReportRequest request)
        {
            var reportRequest = new GetAgentTeamInfoRequest
            {
                AgentId = id,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentTeamInfoAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new TeamInfoReportModel
                {
                    Username = x.Username,
                    RegisteredOn = x.RegisteredOn,
                    Contribution = x.Contribution
                }).ToArray()
            };
        }

        [HttpGet("dividends")]
        [Description("Get agent dividends report")]
        public async Task<PagedResponse<AgentDividendReportModel>>
            GetAgentDividendsAsync([FromQuery] GetAgentDividendsReportRequest request)
        {
            var reportRequest = new GetAgentDividendsRequest
            {
                BrandId = request.BrandId,
                AgentId = request.AgentId,
                Username = request.Username,
                Type = (short?)request.Type,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentDividendsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new AgentDividendReportModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    DepositAmount = x.DepositAmount,
                    WithdrawalAmount = x.WithdrawalAmount,
                    AdjustmentAmount = x.AdjustmentAmount,
                    BonusAmount = x.BonusAmount,
                    BetAmount = x.BetAmount,
                    ValidBetAmount = x.ValidBetAmount,
                    Turnover = x.Turnover,
                    WinLossAmount = x.WinLossAmount,
                    NetProfit = x.NetProfit,
                    AgentDividendAmount = x.AgentDividendAmount,
                    FromPlayerAgentDividendAmount = x.FromPlayerAgentDividendAmount,
                    FromTeamAgentDividendAmount = x.FromTeamAgentDividendAmount
                }).ToArray()
            };
        }

        [HttpGet("dividendDetails")]
        [Description("Get agent dividend details report")]
        public async Task<PagedResponse<AgentDividendDetailReportModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetAgentDividendDetailsReportRequest request)
        {
            var reportRequest = new GetAgentDividendDetailsRequest
            {
                AgentId = request.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentDividendDetailsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new AgentDividendDetailReportModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    SettlementDate = x.SettlementDate,
                    SettlementDateFrom = x.SettlementDateFrom,
                    SettlementDateTo = x.SettlementDateTo,
                    LiveDealerWinLoss = x.LiveDealerWinLoss,
                    SlotWinLoss = x.SlotWinLoss,
                    LotteryWinLoss = x.LotteryWinLoss,
                    CardGameWinLoss = x.CardGameWinLoss,
                    SportsWinLoss = x.SportsWinLoss,
                    VirtualSportsWinLoss = x.VirtualSportsWinLoss,
                    WinLossAmount = x.WinLossAmount,
                    AgentDividendAmount = x.AgentDividendAmount,
                    AgentDividendPercentage = x.AgentDividendPercentage,
                    ExpenditureAmount = x.ExpenditureAmount
                }).ToArray()
            };
        }

        [HttpGet("dividendTeamDetails")]
        [Description("Get agent dividend team details report")]
        public async Task<PagedResponse<AgentDividendTeamDetailReportModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetAgentDividendTeamDetailsReportRequest request)
        {
            var reportRequest = new GetAgentDividendTeamDetailsRequest
            {
                AgentId = request.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentDividendTeamDetailsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new AgentDividendTeamDetailReportModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    SettlementDate = x.SettlementDate,
                    SettlementDateFrom = x.SettlementDateFrom,
                    SettlementDateTo = x.SettlementDateTo,
                    LiveDealerWinLoss = x.LiveDealerWinLoss,
                    SlotWinLoss = x.SlotWinLoss,
                    LotteryWinLoss = x.LotteryWinLoss,
                    CardGameWinLoss = x.CardGameWinLoss,
                    SportsWinLoss = x.SportsWinLoss,
                    VirtualSportsWinLoss = x.VirtualSportsWinLoss,
                    WinLossAmount = x.WinLossAmount,
                    AgentDividendAmount = x.AgentDividendAmount,
                    AgentDividendPercentage = x.AgentDividendPercentage,
                    ExpenditureAmount = x.ExpenditureAmount,
                    BonusAmount = x.BonusAmount
                }).ToArray()
            };
        }

        [HttpGet("{id}/winLoss")]
        [Description("Get agent player Win/Loss report")]
        public async Task<PagedResponse<AgentPlayerWinLossReportModel>> GetAgentPlayerWinLossAsync(
            Guid id,
            [FromQuery] GetAgentPlayerWinLossReportRequest request)
        {
            var reportRequest = new GetAgentPlayerWinLossRequest
            {
                AgentId = id,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentPlayerWinLossAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new AgentPlayerWinLossReportModel
                {
                    Username = x.Username,
                    LiveDealerWinLossAmount = x.LiveDealerWinLossAmount,
                    SlotWinLossAmount = x.SlotWinLossAmount,
                    LotteryWinLossAmount = x.LotteryWinLossAmount,
                    CardGameWinLossAmount = x.CardGameWinLossAmount,
                    SportsWinLossAmount = x.SportsWinLossAmount,
                    VirtualSportsWinLossAmount = x.VirtualSportsWinLossAmount
                }).ToArray()
            };
        }

        [HttpGet("exportAgentContributions")]
        public async Task<IActionResult> ExportAgentContributionsFileAsync(
            [FromQuery] ExportAgentContributionsRequest request)
            => File(
                await _client.ExportAgentContributionsAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AgentContributions.xlsx");

        [HttpGet("exportAgentDividends")]
        public async Task<IActionResult> ExportAgentDividendsFileAsync(
            [FromQuery] ExportAgentDividendsRequest request)
            => File(
                await _client.ExportAgentDividendsAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AgentDividends.xlsx");

        [HttpGet("exportAgentDividendsTrialCalculation")]
        [Description("export agent dividends trial calculation")]
        public async Task<IActionResult> ExportAgentDividendsTrialCalculationAsync(
            [FromQuery] ExportAgentDividendsTrialCalculationRequest request)
            => File(
                await _client.ExportAgentDividendsTrialCalculationAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AgentDividendsTrialCalculation.xlsx");
    }
}