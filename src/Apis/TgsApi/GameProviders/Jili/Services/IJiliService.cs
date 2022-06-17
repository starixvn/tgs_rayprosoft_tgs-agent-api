using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public interface IJiliService
{
    ValueTask<JiliConfiguration> GetConfigurationAsync();

    ValueTask<GameActionData> GetGameActionsByExternalRoundIdAsync(
        Guid gameProviderId,
        string playerCode,
        string externalRoundId,
        string externalGameId,
        PlatformType platformType);

    ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
        Guid gameProviderId,
        Guid gameProviderConfigurationId,
        PlatformType platformType);

    ValueTask<Guid> GetGameProviderIdAsync();
}
