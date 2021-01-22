using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Bet.DataInitializers
{
    partial class DataInitializer
    {
        internal record Brand(Guid Id, string Code, Guid ResellerId, string ResellerCode, Guid LicenseeId,
            string LicenseeCode);

        internal record Player(string PlayerCode, Guid WalletId, string CurrencyCode, PlayerType Type, Brand Brand);

        internal record GameProvider(Guid Id, string Code, string Name, Guid GameProviderConfigurationId, Game[] Games);

        internal record Game(int Id, string Name, GameType Type, PlatformType PlatformType);
    }
}