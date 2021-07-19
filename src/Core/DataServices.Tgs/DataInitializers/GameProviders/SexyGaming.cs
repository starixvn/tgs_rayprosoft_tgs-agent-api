// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.SexyGaming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record SexyGamingGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            private readonly Dictionary<int, GameProviderBetLimit> BetLimitMapper =
                new Dictionary<int, GameProviderBetLimit>
                {
                    {280301, new(GuidExtensions.NewSequentialGuid(), "CNY", 5, 500, 500)},
                    {280302, new(GuidExtensions.NewSequentialGuid(), "CNY", 10, 1000, 1000)},
                    {280303, new(GuidExtensions.NewSequentialGuid(), "CNY", 20, 2500, 2500)},
                    {282202, new(GuidExtensions.NewSequentialGuid(), "EUR", 1, 500, 500)},
                    {282204, new(GuidExtensions.NewSequentialGuid(), "EUR", 10, 2500, 2500)},
                    {282205, new(GuidExtensions.NewSequentialGuid(), "EUR", 10, 5000, 5000)},
                    {280605, new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 50, 5000, 5000)},
                    {280606, new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 100, 10000, 10000)},
                    {280607, new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 200, 20000, 20000)},
                    {281301, new(GuidExtensions.NewSequentialGuid(), "IDR", 5000, 500000, 500000)},     //PTI
                    {281302, new(GuidExtensions.NewSequentialGuid(), "IDR", 5000, 50000000, 50000000)}, //PTI
                    {300403, new(GuidExtensions.NewSequentialGuid(), "JPY", 200, 50000, 50000)},
                    {280801, new(GuidExtensions.NewSequentialGuid(), "KRW", 5000, 100000, 100000)},
                    {280802, new(GuidExtensions.NewSequentialGuid(), "KRW", 100000, 10000000, 10000000)},
                    {280803, new(GuidExtensions.NewSequentialGuid(), "KRW", 200000, 20000000, 20000000)},
                    {280101, new(GuidExtensions.NewSequentialGuid(), "MYR", 1, 250, 250)},
                    {280102, new(GuidExtensions.NewSequentialGuid(), "MYR", 2, 500, 500)},
                    {280103, new(GuidExtensions.NewSequentialGuid(), "MYR", 5, 1000, 1000)},
                    {280908, new(GuidExtensions.NewSequentialGuid(), "THB", 100, 20000, 20000)},
                    {280909, new(GuidExtensions.NewSequentialGuid(), "THB", 500, 50000, 50000)},
                    {280910, new(GuidExtensions.NewSequentialGuid(), "THB", 1000, 100000, 100000)},
                    {280201, new(GuidExtensions.NewSequentialGuid(), "HKD", 2, 500, 500)},
                    {280203, new(GuidExtensions.NewSequentialGuid(), "HKD", 10, 20000, 20000)},
                    {280204, new(GuidExtensions.NewSequentialGuid(), "HKD", 20, 4000, 4000)},
                    {280205, new(GuidExtensions.NewSequentialGuid(), "HKD", 40, 10000, 10000)},
                    {280206, new(GuidExtensions.NewSequentialGuid(), "HKD", 5, 5000, 5000)},
                    {280701, new(GuidExtensions.NewSequentialGuid(), "USD", 1, 100, 100)},
                    {280702, new(GuidExtensions.NewSequentialGuid(), "USD", 1, 500, 500)},
                    {280703, new(GuidExtensions.NewSequentialGuid(), "USD", 5, 200, 200)},
                    {280704, new(GuidExtensions.NewSequentialGuid(), "USD", 5, 1000, 1000)},
                    {280705, new(GuidExtensions.NewSequentialGuid(), "USD", 10, 500, 500)},
                    {281101, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 10, 500, 500)},
                    {281102, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 20, 1000, 1000)},
                    {281103, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 50, 2500, 2500)},
                    {281104, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 100, 5000, 5000)},
                    {281105, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 200, 10000, 10000)},
                    {281401, new(GuidExtensions.NewSequentialGuid(), "VND", 10000, 500000, 500000)} //PTV
                };

            public SexyGamingGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.SexyGaming)
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

            Guid IGameProvider.Id => new("EADD378A-3BB9-6186-3815-563E220C464F");

            string IGameProvider.Code => GameProviderConst.SexyGaming;

            string IGameProvider.Name => "SexyGaming";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new SexyGamingGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"),
                new USDCurrency("USD"),
                new VND1000Currency("VND"),
                new VNDCurrency("PTV"),
                new IDR1000Currency("IDR"),
                new IDRCurrency("PTI"),
                new JPYCurrency("JPY"),
                new KRWCurrency("KRW"),
                new MYRCurrency("MYR"),
                new THBCurrency("THB"),
                new EURCurrency("EUR"),
                new HKDCurrency("HKD")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"),
                new CNLanguage("cn"),
                new JPLanguage("jp"),
                new THLanguage("th"),
                new VNLanguage("vn")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.TransferWallet => false;

            ushort IGameProvider.UniqueId => 5;

            GameProviderBetLimit[] IGameProvider.BetLimits => BetLimitMapper.Select(x => x.Value).ToArray();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new SexyGamingConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-sg") : new("http://localhost:62330"),
                    ApiUrl = new("https://tttint.onlinegames22.com/"),
                    AgentId = "tgs",
                    Cert = "OFIHXL0mfK8AcohvyTs",
                    BetLimit = new SexyGamingBetLimit
                    {
                        SexybcrtMapper = BetLimitMapper.ToDictionary(o => o.Key, o => o.Value.CurrencyCode)
                    }
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.SexyGaming, "SexyGaming")
            };
        }

        internal record SexyGamingGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("BE8B70A3-A8FA-ECEF-BFAD-7A3455B9EF0F");

            string IGameProviderConfiguration.Name => "Configuration SexyGaming";

            string IGameProviderConfiguration.ClientId => "SexyGaming-PG";

            string IGameProviderConfiguration.SecretKey => "dfqqxrchghagdhmfnmzttxgzsfockhmm";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}