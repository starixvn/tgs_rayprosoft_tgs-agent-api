using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Services
{
    public interface IDreamGameService
    {
        ValueTask<bool> ExistGameActionExternalIdAsync(Guid gameProviderId, string externalTransactionId);

        ValueTask<bool> GetAutoCloseRoundAsync();

        ValueTask<DreamGameConfiguration> GetDreamGameConfigurationAsync();

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<GameActionExternalIds> GetGameActionExternalIdByCancelTransactionAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<OrderInfo[]> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId);

        ValueTask<CachedGame> GetGameAsync();

        ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId);

        ValueTask<Guid> GetGameProviderIdAsync();

        string GetHashMD5(string agentName, string ApiKey);

        ValueTask<PlayerLoginRecordModel> GetPlayerLoginLastAsync(string playerCode);

        ValueTask<OrderInfo[]> GetUnsettleAsync(Guid gameProviderId);
    }
}
