using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.GameProvider;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.GameProvider;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameProviderReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public GameProviderReportController(IReportClient client) => _client = client;

        [HttpGet]
        [Description("Get game provider round aggregate")]
        public async Task<GetGameProviderRoundsReportResponse> GetGameProviderRoundsAsync(
            [FromQuery] GetGameProviderRoundsReportRequest request)
        {
            var reportRequest = new GetGameProviderRoundsRequest
            {
                From = request.From,
                To = request.To,
                BrandId = request.BrandId,
                GameProviderCode = request.GameProviderCode,
                GameType = (short?)request.GameType,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetGameProviderRoundsAsync(reportRequest);

            return new()
            {
                Datas = new PagedResponse<GameProviderRoundsReportModel>
                {
                    TotalCount = response.Datas.TotalCount,
                    Items = response.Datas.Items.Select(x => new GameProviderRoundsReportModel
                    {
                        GameProviderId = x.GameProviderId,
                        GameProviderName = x.GameProviderName,
                        CurrencyCode = x.CurrencyCode,
                        TransactionCount = x.TransactionCount,
                        BetAmount = x.BetAmount,
                        ValidBetAmount = x.ValidBetAmount,
                        OriginalTurnover = x.OriginalTurnover,
                        CompanyWinLoss = x.CompanyWinLoss
                    }).ToArray()
                },
                Totals = response.Totals.Select(x => new GameProviderRoundTotalReportModel
                {
                    CurrencyCode = x.CurrencyCode,
                    TransactionCount = x.TransactionCount,
                    BetAmount = x.BetAmount,
                    ValidBetAmount = x.ValidBetAmount,
                    OriginalTurnover = x.OriginalTurnover,
                    CompanyWinLoss = x.CompanyWinLoss
                }).ToArray()
            };
        }
    }
}