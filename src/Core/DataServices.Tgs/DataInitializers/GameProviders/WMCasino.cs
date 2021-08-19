// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.WMCasino;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record WMCasinoGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public WMCasinoGameProvider(IWebHostEnvironment env)
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

            Guid IGameProvider.Id => new("D917ED1E-AEB5-430C-96EB-473B186E9D05");

            string IGameProvider.Code => GameProviderConst.WMCasino;

            string IGameProvider.Name => "WM Casino";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new WMCasinoGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new USDCurrency(),
                new EURCurrency(),
                new MYRCurrency(),
                new KRWCurrency(),
                new INRCurrency(),
                new CNYCurrency(),
                new THBCurrency(),
                new IDRCurrency(),
                new IDR1000Currency(),
                new VNDCurrency(),
                new VND1000Currency()
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new CNLanguage("0"),
                new USLanguage("1"),
                new THLanguage("2"),
                new VNLanguage("3"),
                new JPLanguage("4"),
                new KRLanguage("5"),
                new IDLanguage("6"),
                new TWLanguage("9")
            };

            IGame[] IGameProvider.Games => _games;

            ushort IGameProvider.UniqueId => 17;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.SupportTestPlayer => true;

            bool IGameProvider.TransferWallet => false;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new WMCasinoConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-wm") : new("http://localhost:62330"),
                    ApiUrl = new("https://api.a45.me/api/wallet/Gateway.php"),
                    MaxResultCount = 25,
                    BrandMerchants = new WMCasinoBrandMerchant[]
                    {
                        new()
                        {
                            BrandCode = "Default",
                            Agents = new WMCasinoAgent[]
                            {
                                new()
                                {
                                    AgentId = "hpgtwapi",
                                    Signature = "97ffc7ab86c2f54be533e90da6f6ab9f",
                                    CurrencyCode = "CNY"
                                }
                            }
                        }
                    }
                };

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.WMCasino, GameProviderConst.WMCasino)
            };
        }

        internal class WMCasinoGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _Endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("98EA2F65-F6DE-4279-A303-9A1E7DA890F3");

            string IGameProviderConfiguration.Name => "Configuration WMCasino";

            string IGameProviderConfiguration.ClientId => "WMCasino-PG";

            string IGameProviderConfiguration.SecretKey => "jcjiwrvpgwooyuevfrqlvbhsxrdobatn";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}