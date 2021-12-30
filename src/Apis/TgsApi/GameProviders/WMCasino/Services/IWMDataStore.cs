using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMDataStore
    {
        ValueTask AddBetDetailRequestInfoAsync(string agentId, string roundNumber, BetDetailRequestInfo info);

        ValueTask AddBetResultsToQueueAsync(string batchId, IEnumerable<BetDetail> betDetails);

        ValueTask ApplyBetResultBatchesAsync(IEnumerable<string> batchIds);

        ValueTask<string> GetBatchIdAsync();

        ValueTask<IEnumerable<BetDetailRequestInfo>> GetBetDetailRequestInfosAsync(string agentId, string roundNumber);

        ValueTask<IEnumerable<BetDetail>> GetBetResultsByBatchIdAsync(string batchId);

        ValueTask RemoveBetDetailRequestInfosAsync(string agentId, string roundNumber);

        ValueTask RemoveBetResultsByBatchIdAsync(string batchId);
    }
}
