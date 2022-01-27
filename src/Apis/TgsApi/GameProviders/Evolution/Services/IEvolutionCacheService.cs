using System.Threading.Tasks;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public interface IEvolutionCacheService
    {
        ValueTask AddExternalTransactionIdAsync(string playerCode, string externalTransactionId);

        ValueTask AddSidAsync(string sid, string playerCode);

        Task<bool> ExistExternalTransactionIdAsync(string playerCode, string externalTransactionId);

        ValueTask<string> GetPlayerCodeAsync(string sid);
    }
}
