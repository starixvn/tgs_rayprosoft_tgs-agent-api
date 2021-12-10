// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.AllBet;
using TGS.Core.Models.GameProviders.Settings.Jili;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        internal record JiliGameProvider : IGameProvider
        {
            private const int UniqueId = 22;

            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public JiliGameProvider(IWebHostEnvironment env)
            {
                //_games = LoadGamesFromCsv(GameProviderConst.Jili)
                //    .Select(g =>
                //    {
                //        if (g.EndpointPath.IsNotNullOrEmpty())
                //        {
                //            return g;
                //        }

                //        return (Game)g with { EndpointPath = string.Empty };
                //    })
                //    .ToArray();

                _games = Array.Empty<IGame>();

                _env = env;
            }

            Guid IGameProvider.Id => new("579D6DA3-B0ED-4E3F-BD4A-434A98EFBD68");

            string IGameProvider.Code => GameProviderConst.Jili;

            string IGameProvider.Name => "Jili";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new JiliGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"),
                new USDCurrency("USD"),
                new VND1000Currency("VND2"),
                new IDR1000Currency("IDR2"),
                new JPYCurrency("JPY"),
                new KRWCurrency("KRW"),
                new MYRCurrency("MYR"),
                new THBCurrency("THB"),
                new GBPCurrency("GBP")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en"),
                new CNLanguage("zh_CN"),
                new IDLanguage("id"),
                new JPLanguage("ja"),
                new KRLanguage("ko"),
                new THLanguage("th"),
                new VNLanguage("vi"),
                new TWLanguage("zh_TW")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            ushort IGameProvider.UniqueId => UniqueId;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new JiliConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-jili") : new("http://localhost:62330"),
                    ApiUrl = new("https://uat-wb-api.jlfafafa2.com/singleWallet"),
                    Merchants = new JiliMerchant[]
                    {
                        new()
                        {
                            Brands = new [] { "GPI" },
                            AgentId = "PG_Seamless",
                            AgentKey = "ed7c0450d69bcec7ddf42124ecb7493ef9597083"
                        }
                    }
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.AllBet, "AllBet")
            };
        }

        internal record JiliGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("C16E67AB-797C-4CBE-8400-DDB5466568E8");

            string IGameProviderConfiguration.Name => "Configuration Jili";

            string IGameProviderConfiguration.ClientId => "Jili-PG";

            string IGameProviderConfiguration.SecretKey => "hJwjDSATxZplNbwnnNeLIZcWqFGUThbjZkzlkYQt";

            string IGameProviderConfiguration.DesktopEndpoint => string.Empty;

            string IGameProviderConfiguration.MobileEndpoint => string.Empty;
        }
    }
}
