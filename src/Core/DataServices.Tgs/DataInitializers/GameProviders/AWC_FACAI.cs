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
    internal record AWC_FACAIGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_FACAIGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_FACAI)
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

        Guid IGameProvider.Id => new("3E9D0A02-166D-4D65-9151-2BCAA4788CBA");

        string IGameProvider.Code => GameProviderConst.AWC_FACAI;

        string IGameProvider.Name => "AWC_FACAI";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_FACAIGameProviderConfiguration()
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

        ushort IGameProvider.UniqueId => 35;

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
            new(GameProviderConst.AWC_FACAI, GameProviderConst.AWC_FACAI)
        };
    }

    internal record AWC_FACAIGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("75721160-4766-41B1-A980-59A8CE5AF0A7");

        string IGameProviderConfiguration.Name => "AWC_FACAI-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_FACAI-GPI";

        string IGameProviderConfiguration.SecretKey => "ziiowfkroftgjhyqhvzypphwyyfljeac";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpifacai",
            cert = "Xu6MfhYY0nrORVg4JcK"
        });
    }
}
