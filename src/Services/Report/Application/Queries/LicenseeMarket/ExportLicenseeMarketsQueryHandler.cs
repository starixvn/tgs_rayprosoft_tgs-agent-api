using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.LicenseeMarket;

namespace TGS.Services.Report.Application.Queries.LicenseeMarket
{
    internal class ExportLicenseeMarketsQueryHandler : Handler<ExportLicenseeMarketsRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportLicenseeMarketsQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportLicenseeMarketsRequest request,
            CancellationToken cancellationToken)
        {
            var getMarketsRequest = new GetLicenseeMarketsRequest
            {
                LicenseeId = request.LicenseeId,
                CurrencyCode = request.CurrencyCode,
                From = request.From,
                To = request.To,
                ShowAll = true,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy
            };

            var markets = await _mediator.Send(getMarketsRequest, cancellationToken).ConfigureAwait(false);

            var exportData = markets.Datas?.Items?.Select(x => new ExportLicenseeMarketsModel
            {
                BrandCode = x.BrandCode,
                CurrencyCode = x.CurrencyCode,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BonusAmount = x.BonusAmount,
                BetAmount = x.BetAmount,
                Turnover = x.Turnover,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit
            });

            var resource = new ResourceManager(typeof(Resources.LicenseeMarket));

            var xlsx = exportData.ExportWithLocalization("licenseeMarkets", resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}
