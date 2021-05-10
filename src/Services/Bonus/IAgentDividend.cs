using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;

namespace TGS.Services.Bonus
{
    public interface IAgentDividend
    {
        ITgsRepository Repository { get; }
        Task SentAgentDividend(IEnumerable<AgentDividendBonus> agentDividends, TransactionType transactionType);
        Task UpdateAgentDividendSentOn(IEnumerable<AgentDividendBonus> agentDividends);
    }
}