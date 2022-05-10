// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.Joker;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record JokerGameProvider : IGameProvider
    {
        private const int UniqueId = 26;

        private readonly IGame[] _games;

        public JokerGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.Joker)
                .Select(g
                    => !string.IsNullOrEmpty(g.EndpointPath)
                        ? g
                        : (Game)g with { EndpointPath = string.Empty })
                .ToArray();
        }

        Guid IGameProvider.Id => new("2EBFE118-B36B-40FA-85B5-78363470ADDC");

        string IGameProvider.Code => GameProviderConst.Joker;

        string IGameProvider.Name => "Joker";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new JokerGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new HKDCurrency("HKD"),
            new THBCurrency("THB")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en"),
            new THLanguage("th"),
            new TWLanguage("zh")
        };

        IGame[] IGameProvider.Games => _games;

        bool IGameProvider.AllowKickPlayer => false;

        ushort IGameProvider.UniqueId => UniqueId;

        bool IGameProvider.TransferWallet => true;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new JokerConfiguration
            {
                ServiceEndpointUrl = new Uri("http://localhost:62330"),
                ApiUrl = new("https://w.api788.net"),
                GameLaunchUrl = new Uri("https://www.gwp6868.net/"),
                BrandMerchants = new JokerBrandMerchant[]
                {
                    new()
                    {
                        BrandCode = "Default",
                        Agents =  new JokerAgent[]
                        {
                            new()
                            {
                                AgentId = "TGVY",
                                CurrencyCode = "HKD",
                                SecretKey = "s5ghrn77baoto",
                                Status = true
                            },
                            new()
                            {
                                AgentId = "TGVZ",
                                CurrencyCode = "THB",
                                SecretKey = "khtn84nh78yxs",
                                Status = true
                            },
                        }
                    }
                }
            };

        Dictionary<string, string> IGameProvider.SportTypes => new();

        Dictionary<string, string> IGameProvider.MarketTypes => new();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.Joker, "Joker")
        };
    }

    internal record JokerGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("4A6DA9DE-FF3D-40C8-B5AF-30942B755D28");

        string IGameProviderConfiguration.Name => "Configuration Joker";

        string IGameProviderConfiguration.ClientId => "Joker-PG";

        string IGameProviderConfiguration.SecretKey => "vgRqfzBUMQVXTfufXSbcMJensCelybJACbcYJKNG";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;
    }
}
