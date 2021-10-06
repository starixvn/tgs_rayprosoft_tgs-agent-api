using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Application.Resources;

namespace TGS.Services.Report.Application.Queries.Player
{
    internal class ExportPlayerContributionsQueryHandler : Handler<ExportPlayerContributionsRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportPlayerContributionsQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportPlayerContributionsRequest request,
            CancellationToken cancellationToken)
        {
            var getPlayerContributionsRequest = new GetPlayerContributionsRequest
            {
                BrandId = request.BrandId,
                From = request.From,
                To = request.To,
                Page = 1,
                ShowAll = true
            };
            var resource = new ResourceManager(typeof(PlayerContribution));

            var playerContributions =
                await _mediator.Send(getPlayerContributionsRequest, cancellationToken).ConfigureAwait(false);

            var exportData = playerContributions.Items?.Select(x => new ExportPlayerContributionModel
            {
                Username = x.Username,
                Type = x.Type,
                Level = x.Level,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BonusAmount = x.BonusAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit
            });

            var xlsx = exportData.ExportWithLocalization("playerContributions", resource);
            await using var stream = new MemoryStream();
            xlsx.SaveAs(stream);
            return new MemoryStream(stream.ToArray());
        }
    }
}