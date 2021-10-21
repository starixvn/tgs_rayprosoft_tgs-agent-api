using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Agent
{
    internal class
        ExportAgentDividendsTrialCalculationQueryHandler : Handler<ExportAgentDividendsTrialCalculationRequest,
            MemoryStream>
    {
        private readonly IAgentDividendService _agentDividendService;

        public ExportAgentDividendsTrialCalculationQueryHandler(
            IAgentDividendService agentDividendService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _agentDividendService = agentDividendService;
        }

        public override async Task<MemoryStream> Handle(
            ExportAgentDividendsTrialCalculationRequest request,
            CancellationToken cancellationToken)
        {
            var brand = _agentDividendService.Repository.Brands.AsQueryable().First(x => x.Code == request.BrandCode);

            if (request.From is not null && request.To is not null)
            {
                var from = request.From.Value.ToOffset(new TimeSpan(8, 0, 0));
                var to = request.To.Value.ToOffset(new TimeSpan(8, 0, 0));

                brand.CurrentSettlementFrom = new DateTimeOffset(from.Year,
                    from.Month,
                    from.Day,
                    0,
                    0,
                    0,
                    from.Offset);

                brand.CurrentSettlementTo = new DateTimeOffset(to.Year,
                    to.Month,
                    to.Day,
                    0,
                    0,
                    0,
                    to.Offset);
            }

            var (autoSendDividends, manuelOrNegativeDividends, noSendDividends) =
                await _agentDividendService.GetAgentDividends(brand, 0);

            var exportData = autoSendDividends.Union(manuelOrNegativeDividends).Select(x
                => new AgentDividendsTrialCalculationModel
                {
                    AgentUsername = x.AgentUsername,
                    BrandCode = brand.Code,
                    BrandId = x.BrandId,
                    AgentId = x.AgentId,
                    From = x.From,
                    To = x.To,
                    AgentDividendPercentage = x.AgentDividendPercentage,
                    AgentDividendAmount = x.AgentDividendAmount,
                    NegativeDividendAmount =
                        noSendDividends.Where(n => n.AgentId == x.AgentId).Sum(s => s.AgentDividendAmount),
                    AutoSend = x.AutoSend.ToString()
                });

            var xlsx = exportData.Export("agentDividendsTrialCalculation");

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}