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
    internal record AWC_LUDOGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_LUDOGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_LUDO)
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

        Guid IGameProvider.Id => new("06ddb7b5-1dfb-4160-9112-e7ef9242622d");

        string IGameProvider.Code => GameProviderConst.AWC_LUDO;

        string IGameProvider.Name => "AWC_LUDO";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_LUDOGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new HKDCurrency("HKD"),
            new INRCurrency("INR"),
            new MYRCurrency("MYR"),
            new THBCurrency("THB"),
            new USDCurrency("USD"),
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

        ushort IGameProvider.UniqueId => 30;

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
            new(GameProviderConst.AWC_LUDO, GameProviderConst.AWC_LUDO)
        };
    }

    internal record AWC_LUDOGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("aa079292-d948-41eb-8c00-c2e81fff44dd");

        string IGameProviderConfiguration.Name => "AWC_LUDO-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_LUDO-GPI";

        string IGameProviderConfiguration.SecretKey => "ziiowfkroftgjhyqhvzypphwyyfljeac";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpiludo",
            cert = "wRZ6idEg1YleTSPN3Ra"
        });
    }
}
