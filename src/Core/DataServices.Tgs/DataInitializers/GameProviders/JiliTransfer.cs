// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record JiliTransferGameProvider : IGameProvider
    {
        private const int UniqueId = 22;

        private readonly IGame[] _games;

        public JiliTransferGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.JiliTransfer)
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

        string IGameProvider.Code => GameProviderConst.JiliTransfer;

        string IGameProvider.Name => nameof(GameProviderConst.JiliTransfer);

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new JiliTransferGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new HKDCurrency("HKD"),
            new IDRCurrency("IDR"),
            new PHPCurrency("PHP")
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
            new JiliTransferConfiguration
            {
                ServiceEndpointUrl = new("http://localhost:62330"),
                ApiUrl = new("https://uat-wb-api.jlfafafa2.com"),
                AgentKey = "82af5971ae233eaa003b774be4079e7902ea7ff4"
            };

        Dictionary<string, string> IGameProvider.SportTypes => new();

        Dictionary<string, string> IGameProvider.MarketTypes => new();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.JiliTransfer, GameProviderConst.JiliTransfer)
        };
    }

    internal record JiliTransferGameProviderConfiguration : IGameProviderConfiguration
    {
        Guid IGameProviderConfiguration.Id => new("38b7c5ed-8834-45d8-a3eb-483b21752c54");

        string IGameProviderConfiguration.Name => "JILI-PG_GPI_CNY";

        string IGameProviderConfiguration.ClientId => "JILI-PG_GPI_CNY";

        string IGameProviderConfiguration.SecretKey => "BlWtykWNpfkMrapUPdAlSgPVyGhjQoqz";

        string IGameProviderConfiguration.DesktopEndpoint => string.Empty;

        string IGameProviderConfiguration.MobileEndpoint => string.Empty;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(
          new
          {
              AgentId = "PG_GPI_CNY"
          });
    }
}
