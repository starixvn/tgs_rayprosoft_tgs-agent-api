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
        internal record WGSportsGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public WGSportsGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.WGSports)
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

            Guid IGameProvider.Id => new("E978E58F-E07D-1727-030B-A5CF9208163D");

            string IGameProvider.Code => GameProviderConst.WGSports;

            string IGameProvider.Name => "WGSports Sports";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new WGSportsGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("RMB"),new HKDCurrency("HKD")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new CNLanguage("0"),new TWLanguage("1"),new USLanguage("2"),new JPLanguage("3"),new KRLanguage("4")
            };

            IGame[] IGameProvider.Games => _games;

            ushort IGameProvider.UniqueId => 15;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.SupportTestPlayer => false;

            bool IGameProvider.TransferWallet => true;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new WGSportsConfiguration
            {
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-WGSportsfm") : new("http://localhost:62330"),
                ApiUrl = new("https://api.jsc080.net/index.php"),
                PartnerCode = "TA",
                PartnerKey = "abc1238888888888",
                IV = "cba1238888888888"
            };

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.WGSports, GameProviderConst.WGSports)
            };
        }

        internal class WGSportsGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _Endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("AA0ADEE9-FE0D-D1F8-AF4B-30B04B3D455D");

            string IGameProviderConfiguration.Name => "Configuration WGSports Sports";

            string IGameProviderConfiguration.ClientId => "WGSportsFM-PG";

            string IGameProviderConfiguration.SecretKey => "zfxesctcpfgwklpxksvlckpfxutmcvsj";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}