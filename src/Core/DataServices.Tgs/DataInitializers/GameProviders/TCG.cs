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
        internal record TCGGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public TCGGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.TCG)
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

            Guid IGameProvider.Id => new("BEA0C45F-B94D-4553-41E7-2053767598B5");

            string IGameProvider.Code => GameProviderConst.TCG;

            string IGameProvider.Name => "TCG";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new TCGGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("CNY"),
                new IDRCurrency("IDR"),
                new IDR1000Currency("IDRK"),
                new THBCurrency("THB"),
                new MYRCurrency("MYR"),
                new VNDCurrency("VND"),
                new VND1000Currency("VNDK"),
                new USDCurrency("USD"),
                new HKDCurrency("HKD"),
                new INRCurrency("INR")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new CNLanguage("CN"),
                new TWLanguage("TW"),
                new USLanguage("EN"),
                new THLanguage("TH"),
                new VNLanguage("VI"),
                new IDLanguage("ID"),
                new JPLanguage("JA"),
                new KRLanguage("KO")
            };

            IGame[] IGameProvider.Games => _games;

            ushort IGameProvider.UniqueId => 18;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.SupportTestPlayer => true;

            bool IGameProvider.TransferWallet => false;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new TCGConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new("http://game-tcg") : new("http://localhost:62330")
                };

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new("TCG_SEA", "TCG_SEA 东南亚彩"),
                new("TCG_LOTTO", "TCG LOTTO"),
                new("TCG_LOTTO_VN", "TCG_LOTTO_VN 越南彩")
            };
        }

        internal class TCGGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _Endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("AFD6BE85-7B03-A33C-BB16-6CB0FC1FFEA8");

            string IGameProviderConfiguration.Name => "Configuration TCG";

            string IGameProviderConfiguration.ClientId => "TCG-PG";

            string IGameProviderConfiguration.SecretKey => "sbpehzwnvzghhdhcjgjobdzwxrmmojxu";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}