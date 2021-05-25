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
        internal record SexyGamingProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public SexyGamingProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.SexyGaming)
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

            Guid IGameProvider.Id => new("EADD378A-3BB9-6186-3815-563E220C464F");

            string IGameProvider.Code => GameProviderConst.SexyGaming;

            string IGameProvider.Name => "SexyGaming";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new SexyGamingGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VND1000Currency("VND"), new VNDCurrency("PTV"),
                new IDR1000Currency("IDR"), new IDRCurrency("PTI"), new JPYCurrency("JPY"), new KRWCurrency("KRW"),
                new MYRCurrency("MYR"), new THBCurrency("THB"), new EURCurrency("EUR")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage(), new CNLanguage(), new TWLanguage()
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.TransferWallet => true;

            ushort IGameProvider.UniqueId => 10;

            Dictionary<int, GameProviderBetLimit> BetLimitMapper = new Dictionary<int, GameProviderBetLimit> {
                { 280301, new (GuidExtensions.NewSequentialGuid(), "CNY", 5, 500, 500) },
                { 280302, new (GuidExtensions.NewSequentialGuid(), "CNY", 10, 1000, 1000) },
                { 280303, new (GuidExtensions.NewSequentialGuid(), "CNY", 20, 2500, 2500) },
                { 282202, new (GuidExtensions.NewSequentialGuid(), "EUR", 1, 500, 500) },
                { 282204, new (GuidExtensions.NewSequentialGuid(), "EUR", 10, 2500, 2500) },
                { 282205, new (GuidExtensions.NewSequentialGuid(), "EUR", 10, 5000, 5000) },
                { 280605, new (GuidExtensions.NewSequentialGuid(), "IDR", 50, 5000, 5000) },
                { 280606, new (GuidExtensions.NewSequentialGuid(), "IDR", 100, 10000, 10000) },
                { 280607, new (GuidExtensions.NewSequentialGuid(), "IDR", 200, 20000, 20000) },
                { 281503, new (GuidExtensions.NewSequentialGuid(), "INR", 200, 7500, 7500) },
                { 281504, new (GuidExtensions.NewSequentialGuid(), "INR", 300, 15000, 15000) },
                { 281505, new (GuidExtensions.NewSequentialGuid(), "INR", 600, 30000, 30000) },
                { 300403, new (GuidExtensions.NewSequentialGuid(), "JPY", 200, 50000, 50000) },
                { 280801, new (GuidExtensions.NewSequentialGuid(), "KRW", 5000, 100000, 100000) },
                { 280802, new (GuidExtensions.NewSequentialGuid(), "KRW", 100000, 10000000, 10000000) },
                { 280803, new (GuidExtensions.NewSequentialGuid(), "KRW", 200000, 20000000, 20000000) },
                { 280101, new (GuidExtensions.NewSequentialGuid(), "MYR", 1, 250, 250) },
                { 280102, new (GuidExtensions.NewSequentialGuid(), "MYR", 2, 500, 500) },
                { 280103, new (GuidExtensions.NewSequentialGuid(), "MYR", 5, 1000, 1000) },
                { 280908, new (GuidExtensions.NewSequentialGuid(), "THB", 100, 20000, 20000) },
                { 280909, new (GuidExtensions.NewSequentialGuid(), "THB", 500, 50000, 50000) },
                { 280910, new (GuidExtensions.NewSequentialGuid(), "THB", 1000, 100000, 100000) }
            };

            GameProviderBetLimit[] IGameProvider.BetLimits => BetLimitMapper.Select(x => x.Value).ToArray();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new SexyGamingConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-sexygaming") : new("http://localhost:62330"),
                    ApiUrl = new("https://tttint.onlinegames22.com/"),
                    AgentId = "tgs",
                    Cert = "OFIHXL0mfK8AcohvyTs",
                    BetLimit = new SexyGamingBetLimit()
                    {
                        SexybcrtMapper = BetLimitMapper.ToDictionary(o => o.Key, o => o.Value.Id)
                    }
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
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
