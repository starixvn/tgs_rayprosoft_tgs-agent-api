using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record GameProviderConfigurationData
    {
        public Guid GameProviderId { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public bool AutoCloseRound { get; init; }
        public PlatformType PlatformType { get; init; }
    }
}