using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindActionModelMapper
    {
        ValueTask<BetData> BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source);

        ValueTask<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest source);
    }
}
