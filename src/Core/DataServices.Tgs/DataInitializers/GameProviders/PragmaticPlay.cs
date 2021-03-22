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
        internal record PragmaticPlayGameProvider : IGameProvider
        {
            private static readonly string EndpointPath = "?key={KEY}&stylename={SECURELOGIN}";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public PragmaticPlayGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.PragmaticPlay)
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

            Guid IGameProvider.Id => new("5DACA63E-4265-4791-ADDA-CF6E935D39E0");

            string IGameProvider.Code => GameProviderConst.PragmaticPlay;

            string IGameProvider.Name => "PragmaticPlay";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[] {new PragmaticPlayGameProviderConfiguration()};

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VNDCurrency("VND"), new IDRCurrency("IDR"),
                //new VND1000Currency("VND2"), new IDR1000Currency("IDR2"), 
                new JPYCurrency("JPY"), new EURCurrency("EUR"), new KRWCurrency("KRW"), new MYRCurrency("MYR"),
                new THBCurrency("THB"), new GBPCurrency("GBP")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"), new THLanguage("th"), new JPLanguage("ja"), new IDLanguage("id"),
                new VNLanguage("vi"), new CNLanguage("zh"), new TWLanguage("zt"), new KRLanguage("ko")
            };

            IGame[] IGameProvider.Games => _games;

            short IGameProvider.UniqueId => 3;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new PragmaticPlayConfiguration
            {
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-pragmaticplay") : new("http://localhost:62330"),
                SecureLogin = "WG",
                SecretKey = "E3zwYVhtUeFXG",
                ProviderId = "PragmaticPlay",
                ApiUrl = new("https://api.prerelease-env.biz"),
                GameUrlQueryStringTemplate =
                    "token={TOKEN}&symbol={EXTERNALGAMEID}&technology={TECHNOLOGY}&platform={PLATFORM}&language={CULTURE}",
                DelaySecondsForPlaceBet = 0
            };
        }

        internal class PragmaticPlayGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _Endpoint = "https: //tgp-tw.prerelease-env.biz/gs2c/playGame.do";

            Guid IGameProviderConfiguration.Id => new("455B0CA8-64B1-40A9-8C0A-FCFD3FE895FC");

            string IGameProviderConfiguration.Name => "Configuration PragmaticPlay";

            string IGameProviderConfiguration.ClientId => "PP-WG";

            string IGameProviderConfiguration.SecretKey => "1ofNpUR2B9UzkODEZCU2ugqBfk2mtdNu";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}