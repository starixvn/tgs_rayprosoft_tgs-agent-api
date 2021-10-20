//using System.IO;
//using System.Linq;
//using System.Resources;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using TGS.Services.Report.Application.Models.Agent;
//using TGS.Services.Report.Application.Resources;
//using TGS.Services.Report.Infrastructure.DataService;

//namespace TGS.Services.Report.Application.Queries.Agent
//{
//    internal class
//        ExportAgentDividendsTrialCalculationQueryHandler : Handler<ExportAgentDividendsTrialCalculationRequest,
//            MemoryStream>
//    {
//        private readonly IAgentDividendService _agentDividendService;

//        public ExportAgentDividendsTrialCalculationQueryHandler(
//            IAgentDividendService agentDividendService, IHttpContextAccessor accessor) :
//            base(accessor)
//        {
//            _agentDividendService = agentDividendService;
//        }

//        public override async Task<MemoryStream> Handle(
//            ExportAgentDividendsTrialCalculationRequest request,
//            CancellationToken cancellationToken)
//        {
//            var brnad = _agentDividendService.Repository.Brands.AsQueryable().First(x => x.Code == request.BrandCode);

//            var (autoSendDividends, manuelOrNegativeDividends, _) =
//                await _agentDividendService.GetAgentDividends(brnad, 0);

//            var exportData = agentDividends.Items?.Select(x => new ExportAgentDividendModel
//            {
//                Username = x.Username,
//                DepositAmount = x.DepositAmount,
//                WithdrawalAmount = x.WithdrawalAmount,
//                BonusAmount = x.BonusAmount,
//                AdjustmentAmount = x.AdjustmentAmount,
//                BetAmount = x.BetAmount,
//                ValidBetAmount = x.ValidBetAmount,
//                WinLossAmount = x.WinLossAmount,
//                NetProfit = x.NetProfit,
//                AgentDividendAmount = x.AgentDividendAmount
//            });

//            var resource = new ResourceManager(typeof(AgentDividend));

//            var xlsx = exportData.ExportWithLocalization("agentContributions", resource);

//            await using var stream = new MemoryStream();

//            xlsx.SaveAs(stream);

//            return new MemoryStream(stream.ToArray());
//        }
//    }
//}

