using System;
using System.Threading.Tasks;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services;

public interface IWMCasinoServices
{
    ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByExternalGameActionGroupIdAsync(
        Guid gameProviderId,
        string playerCode, 
        string externalGameActionGroupId);

    ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
        Guid gameProviderId,
        Guid gameProviderConfigurationId, 
        string playerCode, 
        string externalGameId);

    ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataWithDesktopPlatformAsync(
        Guid gameProviderId, 
        Guid gameProviderConfigurationId);
}