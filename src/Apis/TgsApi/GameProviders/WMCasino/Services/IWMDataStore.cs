using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMDataStore
    {
        ValueTask AddBetResultsToQueueAsync(string batchId, IEnumerable<BetDetail> betDetails);

        ValueTask AddPlayerCodeByRoundNumberAsync(string roundNumber, string playerCode);

        ValueTask ApplyBetResultBatchesAsync(string roundNumber);

        ValueTask<string> GetBatchIdAsync();

        ValueTask<IEnumerable<BetDetail>> GetBetResultsByBatchIdAsync(string batchId);
    }
}
