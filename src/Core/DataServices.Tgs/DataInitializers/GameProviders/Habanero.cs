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
        internal record HabaneroGameProvider : IGameProvider
        {
            private const int UniqueId = 2;

            private static readonly string EndpointPath = "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public HabaneroGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.Habanero)
                    .Select(g =>
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                        {
                            return g;
                        }

                        return (Game)g with {EndpointPath = EndpointPath};
                    })
                    .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("DE301075-182B-453F-A316-28E3164B2D06");

            string IGameProvider.Code => GameProviderConst.Habanero;

            string IGameProvider.Name => "Habanero";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new HabaneroGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VNDCurrency("VND"), new IDRCurrency("IDR"),
                new VND1000Currency("VND2"), new IDR1000Currency("IDR2"), new JPYCurrency("JPY"),
                new KRWCurrency("KRW"), new MYRCurrency("MYR"), new THBCurrency("THB"), new GBPCurrency("GBP")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("zh-CN"), new IDLanguage("id"), new JPLanguage("ja"),
                new KRLanguage("ko"), new THLanguage("th"), new VNLanguage("vi"), new TWLanguage("zh-TW")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            short IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new HabaneroConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-habanero") : new("http://localhost:62330"),
                    BrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a",
                    ApiKey = "349F896E-DBC8-4A9D-963D-EFC921AD19FC",
                    ApiUrl = new("https://ws-test.insvr.com/jsonapi"),
                    BetHistoryUrl = new("https://app-test.insvr.com"),
                    DelaySecondsForPlaceBet = 0
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record HabaneroGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://app-test.insvr.com/go.ashx";

            Guid IGameProviderConfiguration.Id => new("92207E3A-96FA-4D90-B713-BD83F023A38B");

            string IGameProviderConfiguration.Name => "Configuration Habanero";

            // this one is defined by game provider and cannot be changed
            string IGameProviderConfiguration.ClientId => "habanero";

            // this one is defined by game provider and cannot be changed
            string IGameProviderConfiguration.SecretKey => "NzrgFf4Bb5KuXy9";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}