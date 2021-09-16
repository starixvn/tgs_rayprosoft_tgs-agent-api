// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.Card365;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record Card365GameProvider : IGameProvider
        {
            private const int _UniqueId = 19;

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

            Guid IGameProvider.Id => new("630E822B-2635-4314-A7E4-5C7057C1A920");

            string IGameProvider.Code => GameProviderConst.Card365;

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

            ushort IGameProvider.UniqueId => _UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new Card365Configuration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-365") : new("http://localhost:62330"),
                    ApiUrl = new("https://g5ky3i.365gaming.cc/api/website/"),
                    Cert = "755C24B710D9F2B4B15DAB2B489CF471",
                    Website = "card365pega",
                    BrandAgents = new[]
                    {
                        new Card365BrandAgent
                        {
                            BrandCode = "Default",
                            AgentId = "card0201"
                        }
                    }
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
            private static readonly string _Endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("5DD6C034-297A-4718-8D89-D6811A46ABED");

            string IGameProviderConfiguration.Name => "Configuration 365";

            string IGameProviderConfiguration.ClientId => "365-PG";

            string IGameProviderConfiguration.SecretKey => "pLDGFVWLJaioBBlPwZUwybkQQyFiwzaCApTRKNiN";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}