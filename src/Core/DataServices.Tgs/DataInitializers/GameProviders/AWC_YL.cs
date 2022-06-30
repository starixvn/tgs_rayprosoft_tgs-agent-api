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
    internal record AWC_YLGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_YLGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_YL)
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

        Guid IGameProvider.Id => new("f1991842-233c-4009-b385-70b8544bf408");

        string IGameProvider.Code => GameProviderConst.AWC_YL;

        string IGameProvider.Name => "AWC_YL";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_YLGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new HKDCurrency("HKD"),
            new IDRCurrency("PTI"),
            new IDR1000Currency("IDR"),
            new INRCurrency("INR"),
            new MYRCurrency("MYR"),
            new PHPCurrency("PHP"),
            new THBCurrency("THB"),
            new USDCurrency("USD"),
            new VNDCurrency("PTV"),
            new VND1000Currency("VND")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en"),
            new CNLanguage("cn"),
            new TWLanguage("cn"),
            new JPLanguage("jp"),
            new THLanguage("th"),
            new VNLanguage("vn"),
        };

        IGame[] IGameProvider.Games => _games;

        bool IGameProvider.AllowKickPlayer => true;

        bool IGameProvider.TransferWallet => true;

        ushort IGameProvider.UniqueId => 32;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new AWCConfiguration
            {
                ServiceEndpointUrl = new Uri("http://localhost:62330"),
                ApiUrl = new Uri("https://tttint.onlinegames22.com/"),
                ApiRecordUrl = new Uri("https://tttint.onlinegames22.com/")
            };

        Dictionary<string, string> IGameProvider.SportTypes => new();

        Dictionary<string, string> IGameProvider.MarketTypes => new();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.AWC_YL, GameProviderConst.AWC_YL)
        };
    }

    internal record AWC_YLGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("3304b836-8de0-4938-91e5-5dc0b87a710e");

        string IGameProviderConfiguration.Name => "AWC_YL-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_YL-GPI";

        string IGameProviderConfiguration.SecretKey => "UgBneXlVvEiTWrUZLyYsmKROodyfCANp";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpiyl",
            cert = "lsOJIO5ejew2DYfjTDV"
        });
    }
}
