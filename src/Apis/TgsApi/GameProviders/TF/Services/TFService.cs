using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public interface ITFService
    {
        ValueTask<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(
            Guid brandId, Guid gameProviderId);

        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        ValueTask<TFConfiguration> GetConfigurationAsync();

        ValueTask<(RoundRecord, GameActionRecord)> GetGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(
            Guid gameProviderId,
            string playerCode,
            string externalTransactionId);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
                                                    Guid gameProviderId,
            Guid gameProviderConfigurationId,
            PlatformType platformType);

        ValueTask<string> GetGameProviderCurrenciesByInternalCodeAsync(
            Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<Guid> GetGameProviderIdByCodeAsync();

        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);

        ValueTask<(RoundRecord, GameActionRecord)> GetSettleGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);
    }

    public class TFService : ITFService
    {
        ValueTask<CachedBrandGameProviderConfiguration> ITFService.GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId) => throw new System.NotImplementedException();

        ValueTask<Guid> ITFService.GetBrandIdByPlayerCodeAsync(string playerCode) => throw new System.NotImplementedException();

        ValueTask<TFConfiguration> ITFService.GetConfigurationAsync() => throw new System.NotImplementedException();

        ValueTask<(RoundRecord, GameActionRecord)> ITFService.GetGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId) => throw new System.NotImplementedException();

        ValueTask<GameActionExternalIds> ITFService.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId) => throw new System.NotImplementedException();

        ValueTask<GameProviderConfigurationData> ITFService.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType) => throw new System.NotImplementedException();

        ValueTask<string> ITFService.GetGameProviderCurrenciesByInternalCodeAsync(Guid gameProviderId, string currencyCode) => throw new System.NotImplementedException();

        ValueTask<Guid> ITFService.GetGameProviderIdAsync() => throw new System.NotImplementedException();

        ValueTask<Guid> ITFService.GetGameProviderIdByCodeAsync() => throw new System.NotImplementedException();

        ValueTask<Player> ITFService.GetPlayerByCodeAsync(string playerCode) => throw new System.NotImplementedException();

        ValueTask<(RoundRecord, GameActionRecord)> ITFService.GetSettleGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId) => throw new System.NotImplementedException();
    }
}
