using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services;

public interface ISkywindActionModelMapper
{
    ValueTask<BetData> BetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, string playerCode, SkywindBetRequest source);

    ValueTask<BetData> RollbackBetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, string playerCode, SkywindRollbackBetRequest source);

    ValueTask<BetData> SettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, string playerCode, SkywindSettleRequest source);
}
