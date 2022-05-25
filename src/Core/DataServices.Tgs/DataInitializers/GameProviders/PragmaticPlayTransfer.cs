// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record PragmaticPlayTransferGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public PragmaticPlayTransferGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.PragmaticPlayTransfer)
                .Select(g => !string.IsNullOrEmpty(g.EndpointPath)
                    ? g
                    : (Game)g with { EndpointPath = string.Empty })
                .ToArray();
        }

        Guid IGameProvider.Id => new("E4ECFA98-B373-4123-87E0-CB9E883D0691");

        string IGameProvider.Code => GameProviderConst.PragmaticPlayTransfer;

        string IGameProvider.Name => "PragmaticPlay";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
            new IGameProviderConfiguration[]
            {
                new PragmaticPlayTransferGameProviderConfiguration()
            };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new USDCurrency("USD"),
            new VNDCurrency("VND"),
            new IDRCurrency("IDR"),
            new VND1000Currency("VND2"),
            new IDR1000Currency("IDR2"),
            new JPYCurrency("JPY"),
            new EURCurrency("EUR"),
            new KRWCurrency("KRW"),
            new MYRCurrency("MYR"),
            new THBCurrency("THB"),
            new GBPCurrency("GBP")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en"),
            new THLanguage("th"),
            new JPLanguage("ja"),
            new IDLanguage("id"),
            new VNLanguage("vi"),
            new CNLanguage("zh"),
            new TWLanguage("zt"),
            new KRLanguage("ko")
        };

        IGame[] IGameProvider.Games => _games;

        ushort IGameProvider.UniqueId => 27;

        bool IGameProvider.TransferWallet => true;

        bool IGameProvider.AllowKickPlayer => true;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() => new PragmaticPlayTransferConfiguration
        {
            ServiceEndpointUrl = null,
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay",
            ApiUrl = new("https://api.prerelease-env.biz")
        };

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.PragmaticPlayTransfer, "PragmaticPlayTransfer")
        };
    }

    internal class PragmaticPlayTransferGameProviderConfiguration : IGameProviderConfiguration
    {
        string IGameProviderConfiguration.ClientId => "PPTF-PG";

        string IGameProviderConfiguration.DesktopEndpoint => string.Empty;

        Guid IGameProviderConfiguration.Id => new("7C308C36-3349-4972-BA32-EBACA4110836");

        string IGameProviderConfiguration.MobileEndpoint => string.Empty;

        string IGameProviderConfiguration.Name => "Configuration PragmaticPlayTransfer";

        string IGameProviderConfiguration.SecretKey => "hizlkuMbSdlGTvlOjlrSplbteTGMMaPkGmFyGxkV";
    }
}
