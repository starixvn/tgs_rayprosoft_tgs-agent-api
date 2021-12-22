using System.Threading.Tasks;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliDataStore
    {
        ValueTask<bool> CheckCancelledRoundAsync(string externalRoundId);

        ValueTask<bool> SetCancelledRoundAsync(string externalRoundId, JiliCancelTransactionRequest request);
    }
}
