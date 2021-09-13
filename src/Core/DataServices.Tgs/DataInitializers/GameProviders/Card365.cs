// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record Card365GameProvider : IGameProvider
        {
            private const int UniqueId = 19;

            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public Card365GameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.Card365)
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

            Guid IGameProvider.Id => new("579D6DA3-B0ED-4E3F-BD4A-434A98EFBD68");

            string IGameProvider.Code => GameProviderConst.AllBet;

            string IGameProvider.Name => "365 Cards";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new Card365GameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new MYRCurrency("1"),
                new HKDCurrency("2"),
                new CNYCurrency("3"),
                new JPYCurrency("4"),
                new IDR1000Currency("6"),
                new USDCurrency("7"),
                new KRWCurrency("8"),
                new THBCurrency("9"),
                new VND1000Currency("11"),
                new INRCurrency("17"),
                new VNDCurrency("55"),
                new IDRCurrency("56")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("EN"),
                new CNLanguage("CN"),
                new THLanguage("TH")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            ushort IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new Card365Configuration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-365") : new("http://localhost:62330"),
                    ApiUrl = new("https://sw2.apidemo.net:8443/")
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.Card365, "365")
            };
        }

        internal record Card365GameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("7D600741-FABD-48AA-99CD-916A3CB19DA4");

            string IGameProviderConfiguration.Name => "Configuration 365";

            string IGameProviderConfiguration.ClientId => "365-PG";

            string IGameProviderConfiguration.SecretKey => "kMbkCVTkkCWXbEImBrTusQFcWudPqzcXoXMgCpVB";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}