// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.BigGaming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record BigGamingGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public BigGamingGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.BigGaming)
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

            Guid IGameProvider.Id => new("D76D09B7-D460-F6DA-E326-A4E9725A8080");

            string IGameProvider.Code => GameProviderConst.BigGaming;

            string IGameProvider.Name => "BigGaming";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new BigGamingGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency(),
                new IDRCurrency(),
                new IDR1000Currency(),
                new MYRCurrency(),
                new THBCurrency(),
                new VNDCurrency(),
                new VND1000Currency()
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en_US"),
                new CNLanguage("zh_CN"),
                new TWLanguage("zh_TW"),
                new THLanguage("th_TH"),
                new IDLanguage("id_ID"),
                new KRLanguage("ko_KR"),
                new JPLanguage("ja_JP"),
                new VNLanguage("vi_VN")
            };

            IGame[] IGameProvider.Games => _games;

            ushort IGameProvider.UniqueId => 8;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new BigGamingConfiguration
            {
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-bg") : new("http://localhost:62330"),
                BrandMerchants = new BigGamingBrandMerchant[]
                {
                    new()
                    {
                        ApiUrl = new("http://openapi-uat.bytechn.com/open-cloud/api/"),
                        Status = true,
                        CurrencyCode = "CNY",
                        AgentLoginIdPassword = "TgsP@ssw0rd",
                        SecretCode = "jk06",
                        SecretKey = "8153503006031672EF300005E5EF6AEF"
                    }
                },
                OppositeBets = new OppositeBetSetting[]
                {
                    new()
                    {
                        // 百家乐[ID: 1]、极速百家乐[ID: 7]、共咪百家乐[ID: 10]
                        Games = new[]
                        {
                            "1",
                            "7 ",
                            "10"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0x10000001", 1), //下注庄赢
                            new("0x10000002", 1), //下注闲赢
                            new("0x10000008", 2), //下注大
                            new("0x10000010", 2)  //下注小
                        }
                    },
                    new()
                    {
                        //轮盘[ID:2]
                        Games = new[]
                        {
                            "2"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                        }
                    },
                    new()
                    {
                        //骰宝[ID: 3]
                        Games = new[]
                        {
                            "3"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                        }
                    },
                    new()
                    {
                        //龙虎[ID: 4]
                        Games = new[]
                        {
                            "4"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0x40000001", 1), //下注龙赢
                            new("0x40000002", 1), //下注虎赢
                            new("0x40000008", 2), //下注龙单
                            new("0x40000010", 2), //下注龙双
                            new("0x40000020", 3), //下注龙红
                            new("0x40000040", 3), //下注龙黑
                            new("0x40000080", 4), //下注虎单
                            new("0x40000100", 4), //下注虎双
                            new("0x40000200", 5), //下注虎红
                            new("0x40000400", 5)  //下注虎黑
                        }
                    },
                    new()
                    {
                        // 红黑大战[ID: 5]
                        Games = new[]
                        {
                            "5"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0x50000001", 1), //红方胜出
                            new("0x50000002", 1)  //黑方胜出
                        }
                    },
                    new()
                    {
                        // 极速骰宝[ID: 6]
                        Games = new[]
                        {
                            "6"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0x60000001", 1),
                            new("0x60000002", 1),
                            new("0x60000004", 2),
                            new("0x60000008", 2)
                        }
                    },
                    new()
                    {
                        // 色碟[ID: 8]
                        Games = new[]
                        {
                            "8"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0x80000001", 1),
                            new("0x80000002", 1)
                        }
                    },
                    new()
                    {
                        // 多彩百家乐[ID: 11]
                        Games = new[]
                        {
                            "11"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0xB0000001", 1),
                            new("0xB0000002", 1)
                        }
                    },
                    new()
                    {
                        //牛牛[ID: 13]
                        Games = new[]
                        {
                            "13"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                        }
                    },
                    new()
                    {
                        //炸金花[ID: 14]
                        Games = new[]
                        {
                            "14"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                            new("0xE0000001", 1),
                            new("0xE0000002", 1)
                        }
                    },
                    new()
                    {
                        //赌场战争[ID: 15]
                        Games = new[]
                        {
                            "15"
                        },
                        Patterns = new OppositeBetSetting.Pattern[]
                        {
                        }
                    }
                }
            };

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.BigGaming, "BigGaming")
            };
        }

        internal class BigGamingGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("A9E3B270-79F5-B455-F476-679D693C82E7");

            string IGameProviderConfiguration.Name => "Configuration BigGaming";

            string IGameProviderConfiguration.ClientId => "BG-PG";

            string IGameProviderConfiguration.SecretKey => "puhtqyhzlipnacxvublifaoldrkwbpth";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}