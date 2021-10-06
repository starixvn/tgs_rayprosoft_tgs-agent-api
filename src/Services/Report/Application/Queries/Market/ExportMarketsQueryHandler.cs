using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.Market;

namespace TGS.Services.Report.Application.Queries.Market
{
    internal class ExportMarketsQueryHandler : Handler<ExportMarketsRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportMarketsQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportMarketsRequest request,
            CancellationToken cancellationToken)
        {
            var getMarketsRequest = new GetMarketsRequest
            {
                BrandId = request.BrandId,
                From = request.From,
                To = request.To,
                ShowAll = true
            };

            var markets = await _mediator.Send(getMarketsRequest, cancellationToken).ConfigureAwait(false);

            var exportData = markets.Items?.Select(x => new ExportMarketModel
            {
                ReportDate = x.ReportDate,
                RegisteredCount = x.RegisteredCount,
                FirstFundInCount = x.FirstFundInCount,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BonusAmount = x.BonusAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.Turnover,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit
            });

            var resource = new ResourceManager(typeof(Resources.Market));

            var xlsx = exportData.ExportWithLocalization("markets", resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}