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
        internal record AllBetGameProvider : IGameProvider
        {
            private const int UniqueId = 13;

            //private static readonly string EndpointPath = "/forwardGame.do?params={PARAS}&key={KEY}";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public AllBetGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.AllBet)
                    .Select(g =>
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                        {
                            return g;
                        }

                        return (Game)g with {EndpointPath = string.Empty};
                    })
                    .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("579D6DA3-B0ED-4E3F-BD4A-434A98EFBD68");

            string IGameProvider.Code => GameProviderConst.AllBet;

            string IGameProvider.Name => "AllBet";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new AllBetGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VND1000Currency("VND2"),
                new IDR1000Currency("IDR2"), new JPYCurrency("JPY"), new KRWCurrency("KRW"), new MYRCurrency("MYR"),
                new THBCurrency("THB"), new GBPCurrency("GBP")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("zh_CN"), new IDLanguage("id"), new JPLanguage("ja"),
                new KRLanguage("ko"), new THLanguage("th"), new VNLanguage("vi"), new TWLanguage("zh_TW")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            ushort IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new AllBetConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-ab") : new("http://localhost:62330"),
                    ApiUrl = new("https://swapi.etwlt.com")
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record AllBetGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("E33902DF-99FF-41CD-8A70-05A3F90A4EE5");

            string IGameProviderConfiguration.Name => "Configuration AllBet";

            string IGameProviderConfiguration.ClientId => "AllBet-PG";

            string IGameProviderConfiguration.SecretKey => "AbofHphhDbPJJcnGMJcLQiXfvsbytGnjToWKkQzI";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}