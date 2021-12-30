using System;
using TGS.Core.Models.GameProviders.Settings.WMCasino;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Models
{
    public record BetDetailRequestInfo(
        Guid GameProviderId,
        WMCasinoAgent Agent,
        BetDetailRequest Request);
}
