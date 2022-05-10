// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.LY;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record LYGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public LYGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.LY)
                .Select(g =>
                {
                    if (g.EndpointPath.IsNotNullOrEmpty())
                    {
                        return g;
                    }

                    return (Game)g with { EndpointPath = string.Empty };
                })
                .ToArray();
        }

        Guid IGameProvider.Id => new("D1E79871-363B-7776-213F-84BA6026E03C");

        string IGameProvider.Code => GameProviderConst.LY;

        string IGameProvider.Name => "LY";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new LYGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new USDCurrency("USD"),
            new VNDCurrency("VND"),
            new IDRCurrency("IDR"),
            new JPYCurrency("JPY"),
            new KRWCurrency("KRW"),
            new MYRCurrency("MYR"),
            new THBCurrency("THB"),
            new EURCurrency("EUR"),
            new HKDCurrency("RMB")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage(),
            new CNLanguage(),
            new TWLanguage()
        };

        IGame[] IGameProvider.Games => _games;

        bool IGameProvider.AllowKickPlayer => false;

        bool IGameProvider.TransferWallet => true;

        ushort IGameProvider.UniqueId => 20;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new LYConfiguration
            {
                ServiceEndpointUrl = new Uri("http://localhost:62330"),
                AmountMaximum = 10000000m,
                AmountFormat = "10,2",
                BrandMerchants = new[]
                {
                    new LYBrandMerchant
                    {
                        BrandCode = "Default",
                        ApiUrl = new("https://wc1-api.wwdlw78.com/channelHandle"),
                        ApiRecordUrl = new("https://wc1-record.wwdlw78.com/getRecordHandle"),
                        Deskey = "706EB65DD6E6DF4C",
                        Md5key = "3E00054B89B31B7A",
                        Status = true,
                        AgentMappers = new LYAgentMapper[]
                        {
                            new()
                            {
                                CurrencyCode = "HKD",
                                AgentId = "200995",
                                Status = true
                            }
                        }
                    }
                }
            };

        Dictionary<string, string> IGameProvider.SportTypes => new();

        Dictionary<string, string> IGameProvider.MarketTypes => new();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.LY, "LY")
        };
    }

    internal record LYGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("A8F6819F-72AE-A4E4-2E6A-34F1CBEAF96A");

        string IGameProviderConfiguration.Name => "Configuration LY";

        string IGameProviderConfiguration.ClientId => "LY-PG";

        string IGameProviderConfiguration.SecretKey => "dntyfkghmpkspxzbfxaaleqghpfqdktj";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;
    }
}
