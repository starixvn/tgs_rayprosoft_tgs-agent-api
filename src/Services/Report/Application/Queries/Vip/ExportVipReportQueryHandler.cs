using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.Vip;
using TGS.Services.Report.Application.Resources;

namespace TGS.Services.Report.Application.Queries.Vip
{
    internal class ExportVipReportQueryHandler : Handler<ExportVipReportRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportVipReportQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportVipReportRequest request,
            CancellationToken cancellationToken)
        {
            var getVipReportRequest = new GetVipReportRequest
            {
                BrandId = request.BrandId,
                Username = request.Username,
                Level = request.Level,
                From = request.From,
                To = request.To,
                ShowAll = true
            };

            var getVipReport =
                await _mediator.Send(getVipReportRequest, cancellationToken).ConfigureAwait(false);

            var exportData = getVipReport.Items?.Select(x => new ExportVipModel
            {
                Username = x.Username,
                Level = x.Level,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                BonusAmount = x.BonusAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                RebateAmount = x.RebateAmount,
                ValidBetAmount = x.Turnover,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit
            });

            var resource = new ResourceManager(typeof(VipReport));

            var xlsx = exportData.ExportWithLocalization("vipReport", resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}