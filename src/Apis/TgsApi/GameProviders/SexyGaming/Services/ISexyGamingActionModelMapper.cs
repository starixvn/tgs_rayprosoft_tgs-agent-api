using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services;

public interface ISexyGamingActionModelMapper
{
    Task<BetData> BetAsync(GameProviderConfigurationData data, string playCode, SexyGamingBet[] source);

    Task<BetData> CancelBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingCancelBet[] source);

    Task<BatchBetData> SettleAsync(GameProviderConfigurationData data, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings);

    Task<BatchBetData> UnsettleAsync(GameProviderConfigurationData data, SexyGamingUnsettle[] source);

    Task<BatchBetData> VoidBetAsync(GameProviderConfigurationData data, SexyGamingVoidBett[] source);

    Task<BatchBetData> VoidSettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidSettle[] source);
}
