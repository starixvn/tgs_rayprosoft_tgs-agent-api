using System;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindService
    {
        ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token);

        ValueTask<bool> GetAutoCloseRoundAsync(Guid gameProviderId);

        ValueTask<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);

        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        ValueTask<SkywindConfiguration> GetConfigurationAsync();

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<Guid> GetGameProviderIdAsync();

        bool IsTest(string playerCode);
    }
}
