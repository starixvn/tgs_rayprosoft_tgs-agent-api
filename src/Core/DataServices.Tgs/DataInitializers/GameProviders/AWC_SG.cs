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
    internal record AWC_SGGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_SGGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_SG)
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

        Guid IGameProvider.Id => new("8b0fe910-b8c6-440a-ae84-2f797b9a312f");

        string IGameProvider.Code => GameProviderConst.AWC_SG;

        string IGameProvider.Name => "AWC_SG";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_SGGameProviderConfiguration()
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

        ushort IGameProvider.UniqueId => 40;

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
            new(GameProviderConst.AWC_SG, GameProviderConst.AWC_SG)
        };
    }

    internal record AWC_SGGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("c844af62-5ab7-4e5d-a8f6-63f688fcaba7");

        string IGameProviderConfiguration.Name => "AWC_SG-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_SG-GPI";

        string IGameProviderConfiguration.SecretKey => "ziiowfkroftgjhyqhvzypphwyyfljeac";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpisg",
            cert = "mFlxPF8LKwuVpDBE1Cw",
            betLimit = new
            {

                SEXYBCRT = new
                {

                    LIVE = new
                    {

                        limitId = new int[] {
                            280301,
                            280302,
                            280303
                        }
                    }
                }
            }
        });
    }
}
