using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.KY;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record KYGameProvider : IGameProvider
        {
            //private static readonly string EndpointPath = "?agent={AGENT}&timestamp={TIMESTAMP}&param={PARAM}";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public KYGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.KY)
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

            Guid IGameProvider.Id => new("A0F49CE0-2C1D-4F50-9FC5-58B610A30BF5");

            string IGameProvider.Code => GameProviderConst.KY;

            string IGameProvider.Name => "KY";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
            {
                new KYGameProviderConfiguration()
            };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("RMB"), new USDCurrency("USD"), new VND1000Currency("VND"),
                new IDR1000Currency("IDR"), new JPYCurrency("JPY"), new KRWCurrency("KRW"), new MYRCurrency("MYR"),
                new THBCurrency("THB"), new EURCurrency("EUR")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage(), new CNLanguage(), new TWLanguage()
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.TransferWallet => true;

            short IGameProvider.UniqueId => 10;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() =>
                new KYConfiguration
                {
                    ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-habanero") : new("http://localhost:62330"),
                    AmountMaximum = 10000000m,
                    AmountFormat = "10,2",
                    BrandMerchants = new[]
                    {
                        new KYBrandMerchant
                        {
                            BrandCode = "Default",
                            ApiUrl = new("https://wc1-api.hddv1.com/channelHandle"),
                            ApiRecordUrl = new("https://wc1-record.hddv1.com/getRecordHandle"),
                            Deskey = "E2F3A416006F84F8",
                            Md5key = "B563DF2A3DE58DFB",
                            Status = true,
                            AgentMappers = new KYAgentMapper[]
                            {
                                new()
                                {
                                    CurrencyCode = "CNY",
                                    AgentId = "72412",
                                    Status = true
                                }
                            }
                        },
                        new KYBrandMerchant
                        {
                            BrandCode = "TGS",
                            ApiUrl = new("https://wc1-api.hddv1.com/channelHandle"),
                            ApiRecordUrl = new("https://wc1-record.hddv1.com/getRecordHandle"),
                            Deskey = "E2F3A416006F84F8",
                            Md5key = "B563DF2A3DE58DFB",
                            Status = false,
                            AgentMappers = new KYAgentMapper[]
                            {
                                new()
                                {
                                    CurrencyCode = "CNY",
                                    AgentId = "72412",
                                    Status = false
                                }
                            }
                        }
                    }
                };

            Dictionary<string, string> IGameProvider.SportTypes => new();

            Dictionary<string, string> IGameProvider.MarketTypes => new();
        }

        internal record KYGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("FA0D7706-F910-4E98-BF71-BD7084140304");

            string IGameProviderConfiguration.Name => "Configuration KY";

            string IGameProviderConfiguration.ClientId => "KY-WG";

            string IGameProviderConfiguration.SecretKey => "WCRhqTigjceMmr65e4P0mz1NaIyCkTWs";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}