using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.Player;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Player;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public PlayerReportController(IReportClient client) => _client = client;

        [HttpGet("contribution")]
        [Description("Get agent contributions")]
        public async Task<PagedResponse<PlayerContributionReportModel>> GetRoundsAsync(
            [FromQuery] GetPlayerContributionsReportRequest request)
        {
            var reportRequest = new GetPlayerContributionsRequest
            {
                BrandId = request.BrandId,
                Username = request.Username,
                Level = request.Level,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetPlayerContributionsAsync(reportRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items.Select(x => new PlayerContributionReportModel
                {
                    Username = x.Username,
                    PlayerCode = x.PlayerCode,
                    Type = (AccountType)x.Type,
                    Level = x.Level,
                    DepositAmount = x.DepositAmount,
                    WithdrawalAmount = x.WithdrawalAmount,
                    AdjustmentAmount = x.AdjustmentAmount,
                    BonusAmount = x.BonusAmount,
                    BetAmount = x.BetAmount,
                    ValidBetAmount = x.ValidBetAmount,
                    Turnover = x.Turnover,
                    WinLossAmount = x.WinLossAmount,
                    NetProfit = x.NetProfit
                }).ToArray()
            };
        }

        [HttpGet("playersAccountChanges")]
        [Description("Get account changes of multiple players")]
        public async Task<PagedResponse<PlayersAccountChangeReportModel>> GetPlayersAccountChangesAsync(
            [FromServices] IReportClient client,
            [FromQuery] GetPlayersAccountChangesReportRequest request)
        {
            var reportRequest = new GetPlayersAccountChangesRequest
            {
                BrandId = request.BrandId,
                Username = request.Username,
                TransactionType = (short?)request.TransactionType,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await client.GetPlayersAccountChangesAsync(reportRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items.Select(x => new PlayersAccountChangeReportModel
                {
                    Username = x.Username,
                    Amount = x.Amount,
                    BeforeBalance = x.BeforeBalance,
                    AfterBalance = x.AfterBalance,
                    TransactionType = (TransactionType)x.TransactionType,
                    RecordedOn = x.RecordedOn,
                    Operator = x.Operator,
                    Description = x.Description
                }).ToArray()
            };
        }

        [HttpGet("exportPlayerContributions")]
        public async Task<IActionResult> ExportPlayerContributionsAsync(
            [FromQuery] ExportPlayerContributionsRequest request)
            => File(
                await _client.ExportPlayerContributionsAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "PlayerContributions.xlsx");
    }
}