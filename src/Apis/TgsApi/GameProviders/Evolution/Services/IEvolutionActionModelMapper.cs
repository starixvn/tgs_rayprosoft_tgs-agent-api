using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Evolution.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public interface IEvolutionActionModelMapper
    {
        BetData Bet(GameProviderConfigurationData data, string playCode, EvolutionDebitRequest source);

        Task<BetData> CancelAsync(GameProviderConfigurationData data, string playCode, EvolutionCancelRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, EvolutionCreditRequest source);
    }
}
