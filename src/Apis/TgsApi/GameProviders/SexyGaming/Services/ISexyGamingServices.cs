using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.GameProviders.Repositories.Models;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services;

public interface ISexyGamingServices
{
    ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token);

    ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

    ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

    ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);

    ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request);
}
