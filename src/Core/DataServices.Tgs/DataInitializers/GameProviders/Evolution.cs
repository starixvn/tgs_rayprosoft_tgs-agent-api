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
        internal record EvolutionGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public EvolutionGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.Evolution)
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

            Guid IGameProvider.Id => new("FC041F82-357F-C4B4-8E37-C2D1701957EE");

            string IGameProvider.Code => GameProviderConst.Evolution;

            string IGameProvider.Name => "Evolution";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new EvolutionGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VNDCurrency("VND"), new VND1000Currency("VN2"),
                new IDRCurrency("IDR"), new IDR1000Currency("ID2"),new JPYCurrency("JPY"), new KRWCurrency("KRW"),
                new MYRCurrency("MYR"), new THBCurrency("THB"), new EURCurrency("EUR"), new HKDCurrency("HKD")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("cn"), new THLanguage("th"), new KRLanguage("ko"),
                new IDLanguage("id"), new VNLanguage("vi"), new TWLanguage("b5")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.TransferWallet => false;

            ushort IGameProvider.UniqueId => 14;


            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new EvolutionConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-evo") : new("http://localhost:62330"),
                    ApiUrl = new("https://babylonqm.uat1.evo-test.com/"),
                    CasinoID = "babylonqm0000001",
                    Ua2Token = "test123",
                    GameHistoryApiToken = "test123",
                    ExternalLobbyApiToken = "test123",
                    Ua2AuthenticationUrl = new("https://babylonqm.uat1.evo-test.com/ua/v1/babylonqm0000001/test123"),
                    OwAuthToken = "I2gcyH6kRTiHYAzLxy1RfH"
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record EvolutionGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("C4B7D077-52CB-80B7-170A-6A6C29BB66D6");

            string IGameProviderConfiguration.Name => "Configuration Evolution";

            string IGameProviderConfiguration.ClientId => "Evolution-PG";

            string IGameProviderConfiguration.SecretKey => "myknltznkezsnslyarhlcgfaznlaxolf";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}
