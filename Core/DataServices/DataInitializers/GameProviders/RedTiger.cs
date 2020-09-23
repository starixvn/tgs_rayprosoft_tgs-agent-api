using System;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal record RedTigerGameProvider : IGameProvider
        {
            private static readonly string EndpointPath = "/{CULTURE}/home";
            private static readonly string QueryString = "?ext_token={TOKEN}";

            private readonly IGame[] _games;

            public RedTigerGameProvider()
            {
                _games = LoadGamesFromCsv(GameProviderConst.RedTiger)
                    .Select(g => 
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                            return g;

                        return (Game)g with { EndpointPath = $"{EndpointPath}{QueryString}" };
                    })
                    .ToArray();
            }

            Guid IGameProvider.Id => new Guid("34136F73-5E12-4823-9CAC-3399E5E72948");

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
        }

        internal record RedTigerGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://gserver-ugs-dev.dopamine-gaming.com/ugs";

            Guid IGameProviderConfiguration.Id => new Guid("B23E82A5-CC79-4F0F-8DEB-1FCD7D5BDD10");

            string IGameProviderConfiguration.Name => "Configuration Dopamine";

            string IGameProviderConfiguration.ClientId => "Dopamine-TGS";

            string IGameProviderConfiguration.SecretKey => "b87j4xy0bPMrpLhAlWCLP05236s261voeWX6nQE057E";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;

            string IGameProviderConfiguration.ConfigurationData => string.Empty;
        }
    }
}