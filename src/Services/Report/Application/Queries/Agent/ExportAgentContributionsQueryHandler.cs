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
    internal class ExportAgentContributionsQueryHandler : Handler<ExportAgentContributionsRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportAgentContributionsQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportAgentContributionsRequest request,
            CancellationToken cancellationToken)
        {
            var getAgentContributionsRequest = new GetAgentContributionsRequest
            {
                BrandId = request.BrandId,
                AgentId = request.AgentId,
                Username = request.Username,
                Type = request.Type,
                From = request.From,
                To = request.To,
                IncludeAgents = request.IncludeAgents,
                ShowAll = true
            };

            var agentContributions =
                await _mediator.Send(getAgentContributionsRequest, cancellationToken).ConfigureAwait(false);

            var exportData = agentContributions.Items?.Select(x => new ExportAgentContributionModel
            {
                Username = x.Username,
                Type = x.Type,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                BonusAmount = x.BonusAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit
            });

            var resource = new ResourceManager(typeof(AgentContribution));

            var xlsx = exportData.ExportWithLocalization("agentContributions", resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}