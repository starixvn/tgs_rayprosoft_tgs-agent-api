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
        internal record AsiaGamingGameProvider : IGameProvider
        {
            private const int UniqueId = 4;

            private static readonly string EndpointPath = "/forwardGame.do?params={PARAS}&key={KEY}";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public AsiaGamingGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.AsiaGaming)
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

            Guid IGameProvider.Id => new("629101FC-3B1C-4719-881B-DCF3C7263C15");

            string IGameProvider.Code => GameProviderConst.AsiaGaming;

            string IGameProvider.Name => "AsiaGaming";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new AsiaGamingGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VND1000Currency("VND2"),
                new IDR1000Currency("IDR2"), new JPYCurrency("JPY"), new KRWCurrency("KRW"), new MYRCurrency("MYR"),
                new THBCurrency("THB"), new GBPCurrency("GBP")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("3"), new CNLanguage("1"), new IDLanguage("11"), new JPLanguage("4"),
                new KRLanguage("5"), new THLanguage("6"), new VNLanguage("8")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            short IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new AsiaGamingConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-ag") : new("http://localhost:62330"),
                    AgentCode = "JH2_AGIN",
                    LockGameTypes = new[] {"ROU"},
                    DesKey = "jh2hnM6K",
                    Md5Key = "jh2bKFbpLZuW",
                    GameHistoryTypes = new[]
                    {
                        new AsiaGamingGameHistoryType
                        {
                            GameTypes = new[] {"BAC", "LBAC", "CBAC"},
                            Descriptions = new Dictionary<string, string>
                            {
                                {"P", "Player"},
                                {"B", "Banker"}
                            }
                        },
                        new AsiaGamingGameHistoryType
                        {
                            GameTypes = new[] {"DT"},
                            Descriptions = new Dictionary<string, string>
                            {
                                {"D", "Dragon"},
                                {"T", "Tiger"}
                            }
                        },
                        new AsiaGamingGameHistoryType
                        {
                            GameTypes = new[] {"NN"},
                            Descriptions = new Dictionary<string, string>
                            {
                                {"F", "First"},
                                {"P1", "Player1"},
                                {"P2", "Player2"},
                                {"P3", "Player3"},
                                {"B", "Banker"}
                            }
                        },
                        new AsiaGamingGameHistoryType
                        {
                            GameTypes = new[] {"ZJH"},
                            Descriptions = new Dictionary<string, string>
                            {
                                {"D", "Dragon"},
                                {"P", "Phoenix"}
                            }
                        },
                        new AsiaGamingGameHistoryType
                        {
                            GameTypes = new[] {"BF"},
                            Descriptions = new Dictionary<string, string>
                            {
                                {"B", "BlackBull"},
                                {"R", "RedBull"}
                            }
                        },
                        new AsiaGamingGameHistoryType
                        {
                            GameTypes = new[] {"BJ", "SG"},
                            Descriptions = new Dictionary<string, string>
                            {
                                {"B", "Banker"},
                                {"P1", "Player1"},
                                {"P2", "Player2"},
                                {"P3", "Player3"},
                                {"P4", "Player4"},
                                {"P5", "Player5"},
                                {"P6", "Player6"},
                                {"P7", "Player7"}
                            }
                        }
                    },
                    LoginApiUrl = new("http://gi.aggdemo.com:81"),
                    ApiUrl = new("https://swapi.etwlt.com")
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record AsiaGamingGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "http://gci.aggdemo.com:81";

            Guid IGameProviderConfiguration.Id => new("C8788CF7-25ED-46B8-ACA8-00C01F7EA662");

            string IGameProviderConfiguration.Name => "Configuration AsiaGaming";

            string IGameProviderConfiguration.ClientId => "AsiaGaming";

            string IGameProviderConfiguration.SecretKey => "fr5q3kaguhyxjEU7bX4Y8P6pTa4PMp4jmBn5sV39";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}