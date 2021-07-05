using System;
using TGS.Core.Models.Enums;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGetPlayerInfo
    {
        public Guid GameProviderConfigurationId { get; init; }
        public string PlayerCode { get; init; }
        public PlatformType PlatformType { get; init; }
    }
}
