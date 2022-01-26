using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.LicenseeMarket;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.LicenseeMarket;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseeMarketReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public LicenseeMarketReportController(IReportClient client) => _client = client;

        [HttpGet("exportLicenseeMarkets")]
        [Description(" Export licensee markets")]
        public async Task<IActionResult> ExportMarketsAsync([FromQuery] ExportLicenseeMarketsRequest request) => File(
            await _client.ExportLicenseeMarketsAsync(request).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "LicenseeMarkets.xlsx");

        [HttpGet]
        [Description("Get licensee markets")]
        public async Task<LicenseeMarketsReportResponse> GetLicenseeMarketsAsync(
            [FromQuery] GetLicenseeMarketsReportRequest request)
        {
            var reportRequest = new GetLicenseeMarketsRequest
            {
                CurrencyCode = request.CurrencyCode,
                CurrencyCodes = request.CurrencyCodes!,
                LicenseeId = request.LicenseeId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetLicenseeMarketsAsync(reportRequest);

            return new()
            {
                Datas = new PagedResponse<LicenseeMarketDataReportModel>
                {
                    TotalCount = response.Datas.TotalCount,
                    Items = response.Datas.Items.Select(x => new LicenseeMarketDataReportModel
                    {
                        BarndId = x.BarndId,
                        BrandCode = x.BrandCode,
                        CurrencyCode = x.CurrencyCode,
                        RegisteredCount = x.RegisteredCount,
                        FirstFundInCount = x.FirstFundInCount,
                        DepositAmount = x.DepositAmount,
                        WithdrawalAmount = x.WithdrawalAmount,
                        AdjustmentAmount = x.AdjustmentAmount,
                        BonusAmount = x.BonusAmount,
                        BetAmount = x.BetAmount,
                        Turnover = x.Turnover,
                        WinLossAmount = x.WinLossAmount,
                        NetProfit = x.NetProfit
                    }).ToArray()
                },
                Totals = response.Totals.Select(x => new LicenseeMarketTotalReportModel
                {
                    CurrencyCode = x.CurrencyCode,
                    RegisteredCount = x.RegisteredCount,
                    FirstFundInCount = x.FirstFundInCount,
                    DepositAmount = x.DepositAmount,
                    WithdrawalAmount = x.WithdrawalAmount,
                    AdjustmentAmount = x.AdjustmentAmount,
                    BonusAmount = x.BonusAmount,
                    BetAmount = x.BetAmount,
                    Turnover = x.Turnover,
                    WinLossAmount = x.WinLossAmount,
                    NetProfit = x.NetProfit
                }).ToArray()
            };
        }
    }
}