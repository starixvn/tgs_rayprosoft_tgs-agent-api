using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMDataStore
    {
        ValueTask AddBetResultsToQueueAsync(IEnumerable<BetDetail> betDetails);

        ValueTask<BetDetail> GetBetResultFromQueueAsync();
    }
}
