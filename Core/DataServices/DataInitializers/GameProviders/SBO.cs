using System;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal record SBOGameProvider : IGameProvider
        {
            private static readonly string EndpointPath = "/{CULTURE}/home";
            private static readonly string QueryString = "?ext_token={TOKEN}";

            private readonly IGame[] _games;

            public SBOGameProvider()
            {
                _games = LoadGamesFromCsv(GameProviderConst.SBOBET)
                    .Select(g => 
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                            return g;

                        return (Game)g with { EndpointPath = $"{EndpointPath}{QueryString}" };
                    })
                    .ToArray();
            }

            Guid IGameProvider.Id => new Guid("8D53C1F2-A718-4204-803F-5B33865175C5");

            string IGameProvider.Code => GameProviderConst.SBOBET;

            string IGameProvider.Name => "SBOBET";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new[] 
            {
                new SBOGameProviderConfiguration() 
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
        }

        internal record SBOGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://gserver-ugs-dev.dopamine-gaming.com/ugs";

            Guid IGameProviderConfiguration.Id => new Guid("CC8AA6E0-5A7F-44BC-B294-5097C3518C37");

            string IGameProviderConfiguration.Name => "Configuration SBO";

            string IGameProviderConfiguration.ClientId => "SBO-WG";

            string IGameProviderConfiguration.SecretKey => "JFYif2fxc6jtOo3fA3JuaItNQ6SAlZAtNYGhgVmZGzm";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;

            string IGameProviderConfiguration.ConfigurationData => string.Empty;
        }

        private static readonly SBOConfiguration SBOConfiguration = new()
        {
            ServiceEndpointUrl = new Uri("https://localhost:62331"),
            CompanyKey = "difjwifjwejfdisdfkidsjfiojwiowej213kiorfj23o5sdfkj9"
        }; 
    }
}