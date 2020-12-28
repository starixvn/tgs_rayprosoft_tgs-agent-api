// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record RedTigerGameProvider : IGameProvider
        {
            private static readonly string EndpointPath = "/{CULTURE}/home";
            private static readonly string QueryString = "?ext_token={TOKEN}";
            private readonly IWebHostEnvironment _env;

            private readonly IGame[] _games;

            public RedTigerGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.RedTiger)
                    .Select(g =>
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                        {
                            return g;
                        }

                        return (Game) g with { EndpointPath = $"{EndpointPath}{QueryString}" };
                    })
                    .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("34136F73-5E12-4823-9CAC-3399E5E72948");

            string IGameProvider.Code => GameProviderConst.RedTiger;

            string IGameProvider.Name => "Red Tiger";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new[]
            {
                new RedTigerGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency(), new VNDCurrency(), new VND1000Currency()
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage(), new CNLanguage()
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AutoClosedRound => true;

            bool IGameProvider.SupportTestPlayer => true;

            short IGameProvider.UniqueId => 999;

            GameProviderBetLimit[] IGameProvider.BetLimits => new GameProviderBetLimit[]
            {
                new(GuidExtensions.NewSequentialGuid(), "CNY", 1, 200, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "CNY", 1, 1000, 1000, "2"),
                new(GuidExtensions.NewSequentialGuid(), "EUR", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "GBP", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "IDR", 1000, 200000, 1000000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "INR", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "JPY", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "KRW", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "MYR", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "SGD", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "THB", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "USD", 1, 1000, 1000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "VND", 1000, 200000, 1000000, "1"),
                new(GuidExtensions.NewSequentialGuid(), "VND_1000", 1, 1000, 1000, "1")
            };

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new RedTigerConfiguration
            {
                ServiceEndpointUrl = new Uri("https://localhost:62331")
            };
        }

        internal record RedTigerGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://gserver-ugs-dev.dopamine-gaming.com/ugs";

            Guid IGameProviderConfiguration.Id => new("B23E82A5-CC79-4F0F-8DEB-1FCD7D5BDD10");

            string IGameProviderConfiguration.Name => "Configuration Dopamine";

            string IGameProviderConfiguration.ClientId => "Dopamine-TGS";

            string IGameProviderConfiguration.SecretKey => "b87j4xy0bPMrpLhAlWCLP05236s261voeWX6nQE057E";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;

            string IGameProviderConfiguration.ConfigurationData => string.Empty;
        }
    }
}