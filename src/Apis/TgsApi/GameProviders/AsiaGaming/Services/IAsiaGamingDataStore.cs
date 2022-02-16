using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Data;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public interface IAsiaGamingDataStore
    {
        public ValueTask<bool> ExistBetResponseTransactionIdAsync(string transactionId);

        public ValueTask<IEnumerable<BetInfo>> GetBetInfosAsync(string roundId);

        public ValueTask RemoveBetInfosAsync(string roundId);

        public ValueTask RemoveBetResponseTransactionIdAsync(string transactionId);

        public ValueTask StoreBetInfoAsync(string roundId, string transactionId, bool settled);

        public ValueTask<bool> StoreBetResponseTransactionIdAsync(string transactionId);
    }
}
