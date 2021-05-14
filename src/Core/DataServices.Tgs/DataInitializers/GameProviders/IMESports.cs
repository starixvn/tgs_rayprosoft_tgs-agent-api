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
        internal record IMESportsGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public IMESportsGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.IMESports)
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

            Guid IGameProvider.Id => new("0A3D1F45-F042-41A3-8581-D7ABEBD74441");

            string IGameProvider.Code => GameProviderConst.IMESports;

            string IGameProvider.Name => "IMESports";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new IMESportsGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"), new USDCurrency("USD"), new VND1000Currency("VND"),
                new IDR1000Currency("IDR"), new JPYCurrency("JPY"), new KRWCurrency("KRW"),
                new THBCurrency("THB"), new MYRCurrency("MYR"), new EURCurrency("EUR")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("EN"), new CNLanguage("ZH-CN"), new IDLanguage("ID"),
                new KRLanguage("KO"), new THLanguage("TH"), new VNLanguage("VI"), new TWLanguage("ZH-HK")
            };

            IGame[] IGameProvider.Games => _games;

            short IGameProvider.UniqueId => 9;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new IMESportsConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-imesports") : new Uri("http://localhost:62330"),
                    MerchantCode = "A33F1A3F3BC8485BA242F9EE61321E90",
                    ProductWallet = 401,
                    ApiUrl = new("https://ex-api-demo-yy.568win.com"),
                    SupportHttps = false
                };
        }

        internal record IMESportsGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("5FB4F65C-6C0D-44A4-A711-E7DB385AA2A8");

            string IGameProviderConfiguration.Name => "Configuration IMESports";

            string IGameProviderConfiguration.ClientId => "IMESports";

            string IGameProviderConfiguration.SecretKey => "VNFFu2kqkWz6PcZcjfkwdZQxnuWdyqn5NDB7gL7skw8Y";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}