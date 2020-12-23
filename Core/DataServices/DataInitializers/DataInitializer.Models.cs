// ReSharper disable InconsistentNaming

using System;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal record CurrencyDefined(string Code, string Name, string NativeName, string Symbol);

        internal record BetLimitDefined(string CurrencyCode, int MinBet, int MaxBet, int MaxBetPerMatch);

        internal record GameProviderBetLimit(Guid Id, string CurrencyCode, int MinBet, int MaxBet, int MaxBetPerMatch,
            string ExternalSetting = "");
    }
}