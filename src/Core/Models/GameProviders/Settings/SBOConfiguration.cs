// ReSharper disable InconsistentNaming

using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record SBOConfiguration : PluggableConfigurationBase
    {
        public string CompanyKey { get; init; }
        public Uri ApiUrl { get; init; }
        public bool EnableCancelFailedTransactionsJob { get; init; } = false;
        public int DelaySecondsForPlaceBet { get; init; } = 0;
        public string[] BypassPlayerCodes { get; init; } = Array.Empty<string>();
    }
}