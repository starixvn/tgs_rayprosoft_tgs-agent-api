using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.BrandGameProvider;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.BrandGameProvider;
using TGS.Services.BackOffice.Application.Models.Aggregate;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandGameProviderReportController : BaseContextController
    {
        private readonly IReportClient _client;
        private readonly IMediator _mediator;

        public BrandGameProviderReportController(IReportClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get brand game provider round aggregate")]
        public async Task<GetBrandGameProviderRoundsReportResponse> GetGameProviderRoundsAsync(
            [FromQuery] GetBrandGameProviderRoundsReportRequest request)
        {
            var reportRequest = new GetBrandGameProviderRoundsRequest
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

            var response = await _client.GetBrandGameProviderRoundsAsync(reportRequest);

            return new()
            {
                Datas = new PagedResponse<BrandGameProviderRoundsReportModel>
                {
                    TotalCount = response.Datas.TotalCount,
                    Items = response.Datas.Items.Select(x => new BrandGameProviderRoundsReportModel
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
                Totals = response.Totals.Select(x => new BrandGameProviderRoundTotalReportModel
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

        [HttpPost("generate")]
        [Description("Generate brand game provider round aggregate")]
        public Task<MessageResponse> GenerateGameProviderAggregateAsync(
            [FromBody] GenerateBrandGameProviderAggregateRequest request)
            => _mediator.Send(request);
    }
}