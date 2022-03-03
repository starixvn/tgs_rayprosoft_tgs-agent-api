using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindService
    {
        ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token);

        ValueTask<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);

        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        ValueTask<SkywindConfiguration> GetConfigurationAsync();

        string GetExternalRoundId(string roundId, long eventId);

        string GetExternalTransactionId(string trxId, long eventId, string action);

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<(RoundStatus RoundStatus, int GameId)?> GetRoundStatusAndGameIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, string externalTransactionId);

        bool IsTest(string playerCode);
    }
}
