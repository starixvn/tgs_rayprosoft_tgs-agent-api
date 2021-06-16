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
        internal record SkywindGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public SkywindGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.Skywind)
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

            Guid IGameProvider.Id => new("CC56D5E4-52E2-C8E2-12BA-A48DC0AD17F7");

            string IGameProvider.Code => GameProviderConst.Skywind;

            string IGameProvider.Name => "Skywind";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new SkywindGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VNDCurrency("VND"),
                new IDRCurrency("IDR"), new JPYCurrency("JPY"), new KRWCurrency("KRW"),
                new MYRCurrency("MYR"), new THBCurrency("THB"), new EURCurrency("EUR"),
                new HKDCurrency("HKD"), new GBPCurrency("GBP"), new SGDCurrency("SGD"),
                new INRCurrency("INR")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("zh-cn"), new TWLanguage("zh-tw"), new JPLanguage("ja"), new KRLanguage("ko"),
                new VNLanguage("vi"), new THLanguage("th"), new IDLanguage("id")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.TransferWallet => false;

            ushort IGameProvider.UniqueId => 12;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new SkywindConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-sg") : new("http://localhost:62330"),
                    ApiUrl = new("https://api.gcpstg.m27613.com/v1"),
                    MerchId = "sw_PG_merchant_stg",
                    MerchPwd = "tm8jhMq2Sc5DdxJ5",
                    SecretKey = "f841de30-a9eb-49d8-87c7-de878b571afe",
                    Username = "PG_merchant_api_stg",
                    Password = "3VSNMGFvbzBJacJT"
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record SkywindGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("F62443F9-672D-73C4-AAA3-220465462E11");

            string IGameProviderConfiguration.Name => "Configuration Skywind";

            string IGameProviderConfiguration.ClientId => "Skywind-PG";

            string IGameProviderConfiguration.SecretKey => "jpeooyzplajhrcvejptbcjljftdbebvg";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}
