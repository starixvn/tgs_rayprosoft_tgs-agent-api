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
                new CNYCurrency("13"), new USDCurrency("3"), new VND1000Currency("51"), new IDR1000Currency("15"),
                new JPYCurrency("32"), new KRWCurrency("45"), new MYRCurrency("2"), new THBCurrency("4"),
                new GBPCurrency("12")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("cs"), new IDLanguage("id"), new JPLanguage("jp"),
                new KRLanguage("ko"), new THLanguage("th"), new VNLanguage("vn"), new TWLanguage("ch")
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