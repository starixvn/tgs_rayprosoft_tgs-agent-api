using System.Threading.Tasks;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public interface IAsiaGamingDataStore
    {
        public ValueTask<bool> ExistBetResponseTransactionIdAsync(string transactionId);

        public ValueTask RemoveBetResponseTransactionIdAsync(string transactionId);

        public ValueTask<bool> StoreBetResponseTransactionIdAsync(string transactionId);
    }
}
