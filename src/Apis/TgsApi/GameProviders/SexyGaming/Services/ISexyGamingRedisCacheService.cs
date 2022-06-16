using System;
using System.Threading.Tasks;
using TGS.Core.Models.Services.Data;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services;

public interface ISexyGamingRedisCacheService
{
    ValueTask AddCancelBetCacheAsync(SexyGamingCancelBet[] source);

    ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

    ValueTask<string> GetCancelBetPlatformTxIdJoinAsync(SexyGamingBet[] source);

    ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

    ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);
}
