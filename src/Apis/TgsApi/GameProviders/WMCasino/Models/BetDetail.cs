using System;
using TGS.Shared.Game.Models.WMCasino.Common;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Models
{
    public record BetDetail(
        Guid GameProviderId,
        Guid GameProviderConfigurationId,
        string PlayerCode,
        GameBetResult GameBetResult);
}
