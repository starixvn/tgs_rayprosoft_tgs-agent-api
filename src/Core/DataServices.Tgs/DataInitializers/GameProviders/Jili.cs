// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.Jili;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record JiliGameProvider : IGameProvider
    {
        private const int UniqueId = 22;

        private readonly IGame[] _games;

        public JiliGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.Jili)
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

        Guid IGameProvider.Id => new("B957E28C-0E94-40AF-9D37-AB48CDEB5D29");

        string IGameProvider.Code => GameProviderConst.Jili;

        string IGameProvider.Name => nameof(GameProviderConst.Jili);

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new JiliGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new USDCurrency("USD"),
            new VNDCurrency("VND"),
            new VND1000Currency("kVND"),
            new IDRCurrency("IDR"),
            new IDR1000Currency("kIDR"),
            new HKDCurrency("HKD"),
            new JPYCurrency("JPY"),
            new KRWCurrency("KRW"),
            new MYRCurrency("MYR"),
            new THBCurrency("THB"),
            new PHPCurrency("PHP"),
            new INRCurrency("INR")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en-US"),
            new CNLanguage("zh-CN"),
            new IDLanguage("id-ID"),
            new THLanguage("th-TH"),
            new VNLanguage("vi-VN"),
            new TWLanguage("zh-TW")
        };

        IGame[] IGameProvider.Games => _games;

        bool IGameProvider.AllowKickPlayer => false;

        ushort IGameProvider.UniqueId => UniqueId;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new JiliConfiguration
            {
                ServiceEndpointUrl = new("http://localhost:62330"),
                ApiUrl = new("https://uat-wb-api.jlfafafa2.com"),
                ClientId = "Jili-staging",
                SecretKey = "iJXZopyrQl",
                Merchants = new JiliMerchant[]
                {
                    new()
                    {
                        BrandCode = "Default",
                        AgentId = "PG_Seamless",
                        AgentKey = "0d90dda822b6df548460da8f551c1a76c01b6a9e"
                    }
                }
            };

        Dictionary<string, string> IGameProvider.SportTypes => new();

        Dictionary<string, string> IGameProvider.MarketTypes => new();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.Jili, "Jili")
        };
    }

    internal record JiliGameProviderConfiguration : IGameProviderConfiguration
    {
        Guid IGameProviderConfiguration.Id => new("C16E67AB-797C-4CBE-8400-DDB5466568E8");

        string IGameProviderConfiguration.Name => "Configuration Jili";

        string IGameProviderConfiguration.ClientId => "Jili-PG";

        string IGameProviderConfiguration.SecretKey => "hJwjDSATxZplNbwnnNeLIZcWqFGUThbjZkzlkYQt";

        string IGameProviderConfiguration.DesktopEndpoint => string.Empty;

        string IGameProviderConfiguration.MobileEndpoint => string.Empty;
    }
}
