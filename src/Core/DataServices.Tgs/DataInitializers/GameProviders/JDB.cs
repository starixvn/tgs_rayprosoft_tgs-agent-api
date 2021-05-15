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
        internal record JDBGameProvider : IGameProvider
        {
            private const int UniqueId = 7;

            private static readonly string EndpointPath = "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real";
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

                        return (Game)g with {EndpointPath = EndpointPath};
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
                new CNYCurrency("RB"), new USDCurrency("US"), new VNDCurrency("VN"), new IDRCurrency("RP"), new JPYCurrency("JP"), 
                new KRWCurrency("KW"), new MYRCurrency("RM"), new THBCurrency("TB"), new GBPCurrency("GB")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new CNLanguage("zh-CN"), new IDLanguage("id"), new JPLanguage("ja"),
                new KRLanguage("ko"), new THLanguage("th"), new VNLanguage("vi"), new TWLanguage("zh-TW")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            short IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new JDBConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-jdb") : new("http://localhost:62330"),
                    ApiUrl = new("https://ws-test.insvr.com/jsonapi"),
                    ClientDomain = "tgs",
                    MerchantCode = "tgs"
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        private record JDBGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = "https://app-test.insvr.com/go.ashx";

            Guid IGameProviderConfiguration.Id => new("02B92A19-EB3E-40D2-97E9-0283F2AE010C");

            string IGameProviderConfiguration.Name => "Configuration JDB";

            // this one is defined by game provider and cannot be changed
            string IGameProviderConfiguration.ClientId => "jdb";

            // this one is defined by game provider and cannot be changed
            string IGameProviderConfiguration.SecretKey => "NzrgFf4Bb5KuXy9";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}