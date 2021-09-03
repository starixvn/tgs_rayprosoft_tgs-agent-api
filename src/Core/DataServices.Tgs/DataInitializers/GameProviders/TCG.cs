// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.TCG;
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
                        _env.IsDocker() ? new("http://game-tcg") : new("http://localhost:62330"),
                    ApiUrl = new("http://www.connect6play.com/doBusiness.do"),
                    Merchants = new TCGMerchant[] {
                        new TCGMerchant
                        {
                            CurrencyCodev="CNY",
                            MerchantCode="pegacnys",
                            DESKey="EuWEfLac",
                            SHA256Key="h2f69FzdSmMZzrdE",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="HKD",
                            MerchantCode="pegahkds",
                            DESKey="1dNvhUV4",
                            SHA256Key="uC7wC9j2GhQBUQu0",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="IDR_1000",
                            MerchantCode="pegaidrks",
                            DESKey="2xifdSt0",
                            SHA256Key="I2PkSJCT5sI4ZHYX",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="IDR",
                            MerchantCode="pegaidrs",
                            DESKey="XKNDjygF",
                            SHA256Key="9MLfRXnHWeuwVDo4",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="INR",
                            MerchantCode="pegainrs",
                            DESKey="dA60szJG",
                            SHA256Key="x3F9G1CxP0YRXIE",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="MYR",
                            MerchantCode="pegamyrs",
                            DESKey="8pi5m8zH",
                            SHA256Key="NNffZsPcfZDNut9r",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="THB",
                            MerchantCode="pegathbs",
                            DESKey="XE3U5zVJ",
                            SHA256Key="4vzJ3p1sRQTcLswn",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="USD",
                            MerchantCode="pegausds",
                            DESKey="kRxkTF9r",
                            SHA256Key="Vz2tGy1Q7TK0AdAQ",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="VND_1000",
                            MerchantCode="pegavndks",
                            DESKey="PjmKWpPX",
                            SHA256Key="g3kstTENOj5yJWCS",
                            PlayerPassword="TgsPassw0rd"
                        },
                        new TCGMerchant
                        {
                            CurrencyCodev="VND",
                            MerchantCode="pegavnds",
                            DESKey="AiynBxjI",
                            SHA256Key="NH5v1N9XlCnbQjj1",
                            PlayerPassword="TgsPassw0rd"
                        },
                    }

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