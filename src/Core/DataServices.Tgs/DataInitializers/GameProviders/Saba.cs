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
        internal record SabaGameProvider : IGameProvider
        {
            private const int UniqueId = 6;

            private static readonly string EndpointPath = "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public SabaGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.Saba)
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

            Guid IGameProvider.Id => new("09D8D244-206B-4030-8110-B2EA90DC9D2F");

            string IGameProvider.Code => GameProviderConst.Saba;

            string IGameProvider.Name => "Saba Sports";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new SabaGameProviderConfiguration()
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
                new SabaConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-saba") : new("http://localhost:62330")
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record SabaGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://app-test.insvr.com/go.ashx";

            Guid IGameProviderConfiguration.Id => new("E3F6CD9F-20BA-4824-86A6-562B135440E1");

            string IGameProviderConfiguration.Name => "Configuration Saba";

            string IGameProviderConfiguration.ClientId => "Saba-WG";

            string IGameProviderConfiguration.SecretKey => "k6JgvabfqlneVTfM9pxmOK14WnmkxQBf";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}