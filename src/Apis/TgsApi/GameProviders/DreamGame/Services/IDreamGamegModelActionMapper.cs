using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Services;

public interface IDreamGamegModelActionMapper
{
    ValueTask<BetData> BetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameTransferRequest request);

    ValueTask<BetData> CancelBetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameInformRequest request);

    ValueTask<BetData> CancelSettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameInformRequest request);

    ValueTask<BetData> ResettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameInformRequest request);

    ValueTask<BetData> SettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameTransferRequest request);
}
