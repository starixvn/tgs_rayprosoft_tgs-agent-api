using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Application.Resources;

namespace TGS.Services.Report.Application.Queries.Agent
{
    internal class ExportAgentDividendsQueryHandler : Handler<ExportAgentDividendsRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportAgentDividendsQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportAgentDividendsRequest request,
            CancellationToken cancellationToken)
        {
            var getAgentDividendsRequest = new GetAgentDividendsRequest
            {
                BrandId = request.BrandId,
                AgentId = request.AgentId,
                Username = request.Username,
                Type = request.Type,
                From = request.From,
                To = request.To,
                ShowAll = true
            };

            var agentDividends =
                await _mediator.Send(getAgentDividendsRequest, cancellationToken).ConfigureAwait(false);

            var exportData = agentDividends.Items?.Select(x => new ExportAgentDividendModel
            {
                Username = x.Username,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                BonusAmount = x.BonusAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit,
                AgentDividendAmount = x.AgentDividendAmount
            });

            var resource = new ResourceManager(typeof(AgentDividend));

            var xlsx = exportData.ExportWithLocalization("agentContributions", resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}