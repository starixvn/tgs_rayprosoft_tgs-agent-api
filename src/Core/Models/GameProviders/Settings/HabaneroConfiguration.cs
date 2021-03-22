using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record HabaneroConfiguration : PluggableConfigurationBase
    {
        public string BrandId { get; init; }
        public string ApiKey { get; init; }
        public Uri ApiUrl { get; init; }
        public Uri BetHistoryUrl { get; init; }
        public int DelaySecondsForPlaceBet { get; init; }
    }
}