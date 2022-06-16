using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.TCG;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services;

public interface ITCGService
{
    ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

    ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

    ValueTask<CachedGame> GetCachedGameAsync(string externalGameId);

    ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, int gameId);

    ValueTask<IEnumerable<CachedGame>> GetCachedGamesAsync(string externalGameId);

    ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode);

    ValueTask<TCGConfiguration> GetConfigurationAsync();

    ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

    ValueTask<(RoundRecord RoundRecord, GameActionRecord GameActionRecord)> GetGameActionsByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

    ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

    ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

    ValueTask<PlayerLoginRecordModel> GetPlayerLatestEnabledLoginRecordAsync(string playerCode);

    ValueTask<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);
}
