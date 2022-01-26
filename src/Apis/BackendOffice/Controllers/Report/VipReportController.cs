using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.Vip;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Vip;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public VipReportController(IReportClient client) => _client = client;

        [HttpGet]
        [Description("Get vips")]
        public async Task<PagedResponse<VipReportModel>> GetVipsAsync([FromQuery] GetVipReportReportRequest request)
        {
            var reportRequest = new GetVipReportRequest
            {
                BrandId = request.BrandId,
                Username = request.Username,
                Level = request.Level,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetVipReportAsync(reportRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items.Select(x => new VipReportModel
                {
                    BrandId = x.BrandId,
                    Username = x.Username,
                    PlayerCode = x.PlayerCode,
                    Level = x.Level,
                    DepositAmount = x.DepositAmount,
                    WithdrawalAmount = x.WithdrawalAmount,
                    BonusAmount = x.BonusAmount,
                    AdjustmentAmount = x.AdjustmentAmount,
                    RebateAmount = x.RebateAmount,
                    BetAmount = x.BetAmount,
                    ValidBetAmount = x.ValidBetAmount,
                    Turnover = x.Turnover,
                    WinLossAmount = x.WinLossAmount,
                    NetProfit = x.NetProfit
                }).ToArray()
            };
        }

        [HttpGet("{username}")]
        [Description("Get vip rebates")]
        public async Task<PagedResponse<VipRebateReportModel>> GetVipRebatesAsync(string username,
            [FromQuery] GetVipRebateReportReportRequest request)
        {
            var reportRequest = new GetVipRebateReportRequest
            {
                BrandId = request.BrandId,
                Username = username,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetVipRebateReportAsync(reportRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items.Select(x => new VipRebateReportModel
                {
                    ReportDate = x.ReportDate,
                    LiveDealerAmount = x.LiveDealerAmount,
                    SlotAmount = x.SlotAmount,
                    LotteryAmount = x.LotteryAmount,
                    CardGameAmount = x.CardGameAmount,
                    SportsAmount = x.SportsAmount,
                    VirtualSportsAmount = x.VirtualSportsAmount,
                    LiveDealerTurnover = x.LiveDealerTurnover,
                    SlotTurnover = x.SlotTurnover,
                    LotteryTurnover = x.LotteryTurnover,
                    CardGameTurnover = x.CardGameTurnover,
                    SportsTurnover = x.SportsTurnover,
                    VirtualSportsTurnover = x.VirtualSportsTurnover,
                    LiveDealerRebatePercentage = x.LiveDealerRebatePercentage,
                    SlotRebatePercentage = x.SlotRebatePercentage,
                    LotteryRebatePercentage = x.LotteryRebatePercentage,
                    CardGameRebatePercentage = x.CardGameRebatePercentage,
                    SportsRebatePercentage = x.SportsRebatePercentage,
                    VirtualSportsRebatePercentage = x.VirtualSportsRebatePercentage
                }).ToArray()
            };
        }

        [HttpGet("exportVips")]
        public async Task<IActionResult> ExportVipsAsync([FromQuery] ExportVipReportRequest request) => File(
            await _client.ExportVipsAsync(request).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Vips.xlsx");
    }
}