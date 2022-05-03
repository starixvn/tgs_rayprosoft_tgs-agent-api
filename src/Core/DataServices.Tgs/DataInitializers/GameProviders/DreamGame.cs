// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.DreamGame;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record DreamGameGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public DreamGameGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.DreamGame)
                .Select(g =>
                {
                    if (g.EndpointPath.IsNotNullOrEmpty())
                    {
                        return g;
                    }

                    return (Game)g with { EndpointPath = string.Empty };
                })
                .ToArray();
        }

        Guid IGameProvider.Id => new("F52B4DF6-4F71-E36E-9625-5065A4D11A28");

        string IGameProvider.Code => GameProviderConst.DreamGame;

        string IGameProvider.Name => "DreamGame";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new DreamGameGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new EURCurrency("EUR"),
            new GBPCurrency("GBP"),
            new IDR1000Currency("IDR2"),
            new INRCurrency("INR"),
            new JPYCurrency("JPY"),
            new KRWCurrency("KRW"),
            new MYRCurrency("MYR"),
            new SGDCurrency("SGD"),
            new THBCurrency("THB"),
            new USDCurrency("USD"),
            new VND1000Currency("VND2"),
            new HKDCurrency("HKD"),
            new PHPCurrency("PHP")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en"),
            new CNLanguage("cn"),
            new TWLanguage("tw"),
            new THLanguage("th"),
            new KRLanguage("kr"),
            new VNLanguage("vi"),
        };

        IGame[] IGameProvider.Games => _games;

        bool IGameProvider.AllowKickPlayer => true;

        bool IGameProvider.TransferWallet => false;

        ushort IGameProvider.UniqueId => 25;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new DreamGameConfiguration
            {
                ServiceEndpointUrl = new Uri("http://localhost:62330"),
                ApiUrl = new Uri(" https://api.dg0.co"),
                VideoGameApiUrl = new Uri("https://ag.dg66.info"),
                ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
                AgentName = "DGTE0105CO",
                PlayerPassword = "TgsP@ssw0rd",
                BetLimits = new[] {
                    new DreamGameBetLimit
                    {
                        Currency="CNY",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="EUR",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="GBP",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="HKD",
                        TableLimit="G"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="IDR_1000",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="INR",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="JPY",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="KRW",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="KRW",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="MYR",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="PHP",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="SGD",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="THB",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="USD",
                        TableLimit="A"
                    },
                    new DreamGameBetLimit
                    {
                        Currency="VND_1000",
                        TableLimit="A"
                    },
                }
            };

        Dictionary<string, string> IGameProvider.SportTypes => new();

        Dictionary<string, string> IGameProvider.MarketTypes => new();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.DreamGame, GameProviderConst.DreamGame)
        };
    }

    internal record DreamGameGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("D57EE18A-D8F8-B0DA-8DBA-10D0945A4735");

        string IGameProviderConfiguration.Name => "Configuration DreamGame";

        string IGameProviderConfiguration.ClientId => "DreamGame-PG";

        string IGameProviderConfiguration.SecretKey => "xjfzmwsdspaulpfdzkegdfxwprudvuhc";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;
    }
}
