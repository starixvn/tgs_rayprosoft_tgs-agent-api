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
        internal record SBOGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public SBOGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.SBOBET)
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

            Guid IGameProvider.Id => new("8D53C1F2-A718-4204-803F-5B33865175C5");

            string IGameProvider.Code => GameProviderConst.SBOBET;

            string IGameProvider.Name => "SBOBET";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new SBOGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VND1000Currency("VND"),
                new IDR1000Currency("IDR"), new JPYCurrency("JPY"), new KRWCurrency("KRW"), new MYRCurrency("MYR"),
                new THBCurrency("THB"), new GBPCurrency("GBP")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("zh-cn"), new IDLanguage("id-id"), new JPLanguage("ja-jp"),
                new KRLanguage("ko-kr"), new THLanguage("th-th"), new VNLanguage("vi-vn"), new TWLanguage("zh-tw")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => true;

            short IGameProvider.UniqueId => 1;

            GameProviderBetLimit[] IGameProvider.BetLimits => new GameProviderBetLimit[]
            {
                new(GuidExtensions.NewSequentialGuid(), "CNY", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "CNY", 1, 200, 1000),
                new(GuidExtensions.NewSequentialGuid(), "CNY", 15, 250, 1000),
                new(GuidExtensions.NewSequentialGuid(), "EUR", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "GBP", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "IDR", 1000, 200000, 1000000),
                new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "INR", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "JPY", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "KRW", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "MYR", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "SGD", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "THB", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "USD", 1, 1000, 1000),
                new(GuidExtensions.NewSequentialGuid(), "VND", 1000, 200000, 1000000),
                new(GuidExtensions.NewSequentialGuid(), "VND_1000", 1, 1000, 1000)
            };

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new SBOConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-sbo") : new Uri("http://localhost:62330"),
                    CompanyKey = "A33F1A3F3BC8485BA242F9EE61321E90",
                    ApiUrl = new("https://ex-api-demo-yy.568win.com"),
                    EnableCancelFailedTransactionsJob = false,
                    DelaySecondsForPlaceBet = 0,
                    BypassPlayerCodes = new[] {"4ud6wst6shvx"}
                };

            Dictionary<string, string> IGameProvider.SportTypes => new()
            {
                {"Soccer", "1"},
                {"Basketball", "2"},
                {"Football", "3"},
                {"Ice Hockey", "4"},
                {"Badminton", "5"}
            };

            Dictionary<string, string> IGameProvider.MarketTypes => new()
            {
                {"ALL", "0"},
                {"Handicap", "1"},
                {"Odd_Even", "2"},
                {"Over_Under", "3"}
            };
        }

        internal record SBOGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("CC8AA6E0-5A7F-44BC-B294-5097C3518C37");

            string IGameProviderConfiguration.Name => "Configuration SBO";

            string IGameProviderConfiguration.ClientId => "SBO-PG";

            string IGameProviderConfiguration.SecretKey => "JFYif2fxc6jtOo3fA3JuaItNQ6SAlZAtNYGhgVmZGzm";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}