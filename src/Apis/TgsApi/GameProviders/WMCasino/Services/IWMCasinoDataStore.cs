using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;
using TGS.Shared.Game.Models.WMCasino.Common;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoDataStore
    {
        ValueTask AddBetDetailRequestInfoAsync(string agentId, string roundNumber, IEnumerable<GameBetResult> gameResults);

        ValueTask AddBetResultsToQueueAsync(string batchId, IEnumerable<BetDetail> betDetails);

        ValueTask ApplyBetResultBatchesAsync(IEnumerable<string> batchIds);

        ValueTask<string> GetBatchIdAsync();

        ValueTask<IEnumerable<GameBetResult>> GetBetDetailRequestInfoAsync(string agentId, string roundNumber);

        ValueTask<long> GetBetDetailRequestInfoCountAsync(string agentId, string roundNumber);

        ValueTask<IEnumerable<BetDetail>> GetBetResultsByBatchIdAsync(string batchId);

        ValueTask RemoveBetDetailRequestInfoAsync(string agentId, string roundNumber);

        ValueTask RemoveBetResultsByBatchIdAsync(string batchId);
    }
}
