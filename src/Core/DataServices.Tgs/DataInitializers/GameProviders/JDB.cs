// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.JDB;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record JDBGameProvider : IGameProvider
        {
            private const int UniqueId = 7;

            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public JDBGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.JDB)
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

            Guid IGameProvider.Id => new("65CC2057-CB52-44AA-8BFF-DBB3D9EBDDDC");

            string IGameProvider.Code => GameProviderConst.JDB;

            string IGameProvider.Name => "JDB";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new JDBGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("RB"),
                new USDCurrency("US"),
                new VNDCurrency("VN"),
                new IDRCurrency("RP"),
                new JPYCurrency("JP"),
                new KRWCurrency("KW"),
                new MYRCurrency("RM"),
                new THBCurrency("TB"),
                new GBPCurrency("GB")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"),
                new CNLanguage("zh-CN"),
                new IDLanguage("id"),
                new JPLanguage("ja"),
                new KRLanguage("ko"),
                new THLanguage("th"),
                new VNLanguage("vi"),
                new TWLanguage("zh-TW")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            ushort IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new JDBConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-jdb") : new("http://localhost:62330"),
                    ApiUrl = new("http://api.jdb711.com"),
                    MoreGame = false,
                    AESIv = "756edb2fc85fe908",
                    AESKey = "4f09f467e6a8fb9f",
                    ClientDomain = "VB3",
                    TimezoneHour = +8,
                    Agents = new JDBAgent[]
                    {
                        new()
                        {
                            CurrencyCode = "CNY",
                            Id = "wgvb3rmb"
                        }
                    }
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.JDB, "JDB")
            };
        }

        private record JDBGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://app-test.insvr.com/go.ashx";

            Guid IGameProviderConfiguration.Id => new("02B92A19-EB3E-40D2-97E9-0283F2AE010C");

            string IGameProviderConfiguration.Name => "Configuration JDB";

            string IGameProviderConfiguration.ClientId => "JDB-PG";

            string IGameProviderConfiguration.SecretKey => "7Wc7cfMdgHEIPq6h88uu8tIXZxZrl3zBI89Ohp5e";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}