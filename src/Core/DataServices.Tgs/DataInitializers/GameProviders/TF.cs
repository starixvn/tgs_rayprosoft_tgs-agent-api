using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.TF;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record TFGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public TFGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.TFGaming)
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

            Guid IGameProvider.Id => new("67D16262-9915-3109-F18D-6043108712AA");

            string IGameProvider.Code => GameProviderConst.TFGaming;

            string IGameProvider.Name => "TF";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new TFGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new HKDCurrency(),
                new IDRCurrency(),
                new IDR1000Currency(),
                new INRCurrency(),
                new JPYCurrency(),
                new KRWCurrency(),                
                new MYRCurrency(),
                new PHPCurrency(),
                new CNYCurrency("RMB"),
                new THBCurrency(),
                new VNDCurrency(),
                new VND1000Currency(),
                new USDCurrency(),
                new SGDCurrency(),
                new EURCurrency(),
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new CNLanguage("zh"),
                new TWLanguage("zh"),
                new THLanguage("th"),
                new VNLanguage("vn"),
                new KRLanguage("kr"),
                new JPLanguage("jp"),
                new USLanguage("en")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => true;

            bool IGameProvider.SupportTestPlayer => false;

            bool IGameProvider.TransferWallet => false;

            ushort IGameProvider.UniqueId => 24;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new SubGameProvider(GameProviderConst.TFGaming,"TF")
            };

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new TFConfiguration
            {
                ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-tf") : new("http://localhost:62330"),
                BrandMerchants = new TFBrandMerchant[]
                {
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "GPI",
                        BrandCurrency = "CNY",
                        PublicToken = "84778f10a5b147a4baf1db8d334ff266",
                        PrivateToken = "00023b230443b2b95ec3750ff324f04f7ff30dfc69c2dde6b4f0563ea8de135c412fc3645d2cb09ecaa3971fdf8ddde90f6b86fc4bbdade18863113de24d42e5b02c3440c339a3893611eefda81e92b8b3f504cedd44963c419f1691bd439927cb1b075700c1d4b9d09dbdef6022a4f196c6438cf1e90107a263309f7ea06072f6aa",
                        PartnerId = "2135"
                    }
                }
            };
        }

        internal class TFGameProviderConfiguration : IGameProviderConfiguration
        {
            string IGameProviderConfiguration.ClientId => "TFGaming-PG";

            string IGameProviderConfiguration.DesktopEndpoint => string.Empty;

            Guid IGameProviderConfiguration.Id => new("E3C3AD82-14D5-B345-AE1B-A9BF9E0C6D79");

            string IGameProviderConfiguration.MobileEndpoint => string.Empty;

            string IGameProviderConfiguration.Name => "Configuration TFGaming";

            string IGameProviderConfiguration.SecretKey => "4feq8YCryYHSGFW";
        }
    }
}
