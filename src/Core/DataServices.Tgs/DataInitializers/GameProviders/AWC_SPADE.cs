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
    internal record AWC_SPADEGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_SPADEGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_SPADE)
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

        Guid IGameProvider.Id => new("50cdf3d9-2407-4e3c-957e-407250017c1d");

        string IGameProvider.Code => GameProviderConst.AWC_SPADE;

        string IGameProvider.Name => "AWC_SPADE";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_SPADEGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new HKDCurrency("HKD"),
            new IDRCurrency("PTI"),
            new IDR1000Currency("IDR"),
            new INRCurrency("INR"),
            new MYRCurrency("MYR"),
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

        ushort IGameProvider.UniqueId => 36;

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
            new(GameProviderConst.AWC_SPADE, GameProviderConst.AWC_SPADE)
        };
    }

    internal record AWC_SPADEGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("d8688a99-0aca-4bf0-b3b0-f4c9af7122aa");

        string IGameProviderConfiguration.Name => "AWC_SPADE-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_SPADE-GPI";

        string IGameProviderConfiguration.SecretKey => "FkiNxitfxBWRXPYQxcfXRoLrXsuwnVVh";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpispade",
            cert = "zZDVjvp2QJtg23veYhP"
        });
    }
}
