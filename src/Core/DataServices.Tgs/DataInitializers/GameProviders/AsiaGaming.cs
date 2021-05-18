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
                    ProductionId = "JH2",
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
                    ApiUrl = new("https://swapi.etwlt.com"),
                    OppositeBets = new OppositeBetSetting[]
                    {
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_AGQ_BAC", "AGIN_AGQ_LBAC", "AGIN_AGQ_CBAC", "AGIN_DSP_BAC",
                                "AGIN_DSP_LBAC", "AGIN_DSP_CBAC", "AGIN_EMA_BAC", "AGIN_EMA_LBAC",
                                "AGIN_EMA_CBAC", "AGIN_LED_BAC", "AGIN_LED_LBAC", "AGIN_LED_CBAC",
                                "AGIN_VIP_BAC", "AGIN_VIP_LBAC", "AGIN_VIP_CBAC", "AGIN_AGNW_BAC",
                                "AGIN_AGNW_LBAC", "AGIN_AGNW_CBAC"
                            },
                            Patterns = new OppositeBetSetting.Pattern[]
                            {
                                new("1", 1), new("2", 1), new("6", 2), new("7", 2)
                            }
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_AGQ_DT", "AGIN_DSP_DT", "AGIN_EMA_DT", "AGIN_LED_DT", "AGIN_VIP_DT",
                                "AGIN_AGNW_DT"
                            },
                            Patterns = new OppositeBetSetting.Pattern[]
                            {
                                new("21", 1), new("22", 1), new("130", 2), new("132", 2), new("131", 3),
                                new("133", 3), new("134", 4), new("136", 4), new("135", 5), new("137", 5)
                            }
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_AGQ_SHB", "AGIN_DSP_SHB", "AGIN_EMA_SHB", "AGIN_LED_SHB",
                                "AGIN_VIP_SHB", "AGIN_AGNW_SHB"
                            },
                            Patterns = new OppositeBetSetting.Pattern[]
                            {
                                new("41", 1), new("42", 1), new("43", 2), new("44", 2)
                            }
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_AGQ_ROU", "AGIN_DSP_ROU", "AGIN_EMA_ROU", "AGIN_LED_ROU",
                                "AGIN_VIP_ROU", "AGIN_AGNW_ROU"
                            },
                            Patterns = new OppositeBetSetting.Pattern[]
                            {
                                new("114", 1), new("115", 1), new("116", 2), new("117", 2), new("118", 2),
                                new("119", 2)
                            }
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_DSP_ZJH", "AGIN_LED_ZJH", "AGIN_VIP_ZJH", "AGIN_AGQ_ZJH",
                                "AGIN_AGNW_ZJH", "AGIN_EMA_ZJH"
                            },
                            Patterns = new OppositeBetSetting.Pattern[] {new("260", 1), new("261", 1)}
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_DSP_ZJH", "AGIN_LED_ZJH", "AGIN_VIP_ZJH", "AGIN_AGQ_ZJH",
                                "AGIN_AGNW_ZJH", "AGIN_EMA_ZJH"
                            },
                            Patterns = new OppositeBetSetting.Pattern[] {new("260", 1), new("261", 1)}
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_DSP_BF", "AGIN_AGQ_BF", "AGIN_LED_BF", "AGIN_VIP_BF", "AGIN_EMA_BF",
                                "AGIN_AGNW_BF"
                            },
                            Patterns = new OppositeBetSetting.Pattern[] {new("270", 1), new("271", 1)}
                        },
                        new()
                        {
                            Games = new[]
                            {
                                "AGIN_DSP_SG", "AGIN_AGQ_SG", "AGIN_LED_SG", "AGIN_VIP_SG", "AGIN_EMA_SG",
                                "AGIN_AGNW_SG"
                            },
                            Patterns = new OppositeBetSetting.Pattern[]
                            {
                                new("320", 1), new("321", 1), new("323", 2), new("324", 2), new("326", 3),
                                new("327", 3)
                            }
                        }
                    }
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