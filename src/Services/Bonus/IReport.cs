using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.DataServices.Report;
using TGS.Core.Models.Bonus;

namespace Bonus
{
    public interface IReport
    {
        IReportRepository Repository { get; }
        Task UpdatePromotionBonusSentOn(IEnumerable<PromotionPlayerBonus> playerBonus);
        Task UpdatePlayerRebateSentOn(IEnumerable<PlayerRebates> playerRebates);
        Task UpdateAgentDividendSentOn(IEnumerable<AgentDividend> agentDividends);
    }
}