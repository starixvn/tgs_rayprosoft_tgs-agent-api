// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record AviaGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public AviaGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.Avia)
                    .Select(g =>
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                        {
                            return g;
                        }

                        return (Game)g with { EndpointPath = string.Empty };
                    })
                    .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("FB9CC4EF-624F-46FB-8F17-A17C273BED9E");

            string IGameProvider.Code => GameProviderConst.Avia;

            string IGameProvider.Name => "Avia";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new AviaGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"),
                new USDCurrency("USD"),
                new VNDCurrency("VND"),
                new VND1000Currency("KVND"),
                new IDRCurrency("IDR"),
                new IDR1000Currency("KIDR"),
                new JPYCurrency("JPY"),
                new KRWCurrency("KRW"),
                new MYRCurrency("MYR"),
                new THBCurrency("THB"),
                new EURCurrency("EUR"),
                new PHPCurrency("PHP"),
                new INRCurrency("INR")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("ENG"),
                new CNLanguage("CHN"),
                new TWLanguage("CHN"),
                new IDLanguage("ID"),
                new JPLanguage("JP"),
                new THLanguage("TH"),
                new KRLanguage("KR"),
                new VNLanguage("VN"),


            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => true;

            bool IGameProvider.TransferWallet => true;

            ushort IGameProvider.UniqueId => 23;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new AviaConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-avia") : new("http://localhost:62330"),
                    AmountMaximum = 10000000m,
                    AmountFormat = "10,2",
                    ApiUrl = new Uri("https://api.aviaapi.vip"),
                    Authorization = "81e2d1a24c684a76a710a70f4900a353",
                    PlayerPassword = "TgsP@ssw0rd"
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.Avia, GameProviderConst.Avia)
            };
        }

        internal record AviaGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("CD18EDD5-B256-ED9B-21E0-098C0F8E8253");

            string IGameProviderConfiguration.Name => "Configuration Avia";

            string IGameProviderConfiguration.ClientId => "Avia-PG";

            string IGameProviderConfiguration.SecretKey => "trlaofrumaddwleppelnkxvbyhwlfabf";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}