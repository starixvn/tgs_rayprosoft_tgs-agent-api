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
    internal record AWC_PGSGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_PGSGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_PGS)
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

        Guid IGameProvider.Id => new("55cc0ab0-e8c9-41b5-89e3-15164b35a766");

        string IGameProvider.Code => GameProviderConst.AWC_PGS;

        string IGameProvider.Name => "AWC_PGS";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_PGSGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new IDR1000Currency("IDR"),
            new INRCurrency("INR"),
            new PHPCurrency("PHP"),
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

        ushort IGameProvider.UniqueId => 34;

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
            new(GameProviderConst.AWC_PGS, GameProviderConst.AWC_PGS)
        };
    }

    internal record AWC_PGSGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("130550a3-fd43-4dd8-9bf4-55b681b4fe43");

        string IGameProviderConfiguration.Name => "AWC_PGS-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_PGS-GPI";

        string IGameProviderConfiguration.SecretKey => "rgtArCCaoJfAGYcDTGqUagpzTqcAkLZs";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpipgs",
            cert = "te2LfPbKlL4f4rQTEBV"
        });
    }
}
