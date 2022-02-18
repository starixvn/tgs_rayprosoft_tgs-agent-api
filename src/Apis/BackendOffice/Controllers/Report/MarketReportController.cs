using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.Market;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Market;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public MarketReportController(IReportClient client) => _client = client;

        [HttpGet]
        [Description("Get market report")]
        public async Task<PagedResponse<MarketReportModel>> GetMarketsAsync([FromQuery] GetMarketsReportRequest request)
        {
            var reportRequest = new GetMarketsRequest
            {
                BrandId = request.BrandId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetMarketsAsync(reportRequest);

            return response.Valid
                ? new()
                {
                    Valid = response.Valid,
                    TotalCount = response.TotalCount,
                    Items = response.Items.Select(x => new MarketReportModel
                    {
                        ReportDate = x.ReportDate,
                        RegisteredCount = x.RegisteredCount,
                        FirstFundInCount = x.FirstFundInCount,
                        DepositAmount = x.DepositAmount,
                        WithdrawalAmount = x.WithdrawalAmount,
                        AdjustmentAmount = x.AdjustmentAmount,
                        BonusAmount = x.BonusAmount,
                        ValidBetAmount = x.ValidBetAmount,
                        Turnover = x.Turnover,
                        WinLossAmount = x.WinLossAmount,
                        NetProfit = x.NetProfit,
                        AdditionalExpenses = x.AdditionalExpenses
                    }).ToArray()
                }
                : new PagedResponse<MarketReportModel>
                {
                    Valid = response.Valid,
                    Message = response.Message
                };
        }

        [HttpGet("additionalExpenses")]
        [Description("Get Additional Expenses")]
        public async Task<GetAdditionalExpensesReportResponse> GetAdditionalExpensesAsync(
            [FromQuery] GetAdditionalExpensesReportRequest request)
        {
            var reportRequest = new GetAdditionalExpensesRequest
            {
                BrandId = request.BrandId,
                ReportDate = request.ReportDate
            };

            var response = await _client.GetAdditionalExpensesAsync(reportRequest);

            return new GetAdditionalExpensesReportResponse
            {
                AdditionalExpenses = response.AdditionalExpenses.Select(x => new AdditionalExpensesReportModel
                {
                    AdditionalExpensesType = (AdditionalExpensesType)x.AdditionalExpensesType,
                    Amount = x.Amount
                }).ToArray()
            };
        }

        [HttpGet("exportMarkets")]
        public async Task<IActionResult> ExportMarketsAsync([FromQuery] ExportMarketsRequest request) => File(
            await _client.ExportMarketsAsync(request).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Markets.xlsx");
    }
}