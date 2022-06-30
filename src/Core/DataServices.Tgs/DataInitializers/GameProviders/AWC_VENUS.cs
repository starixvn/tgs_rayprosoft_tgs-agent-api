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
    internal record AWC_VENUSGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_VENUSGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_VENUS)
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

        Guid IGameProvider.Id => new("9dfc51c5-95d3-460a-8f2d-7cea6ccbb438");

        string IGameProvider.Code => GameProviderConst.AWC_VENUS;

        string IGameProvider.Name => "AWC_VENUS";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_VENUSGameProviderConfiguration()
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
            new USDCurrency("USD")
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

        ushort IGameProvider.UniqueId => 38;

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
            new(GameProviderConst.AWC_VENUS, GameProviderConst.AWC_VENUS)
        };
    }

    internal record AWC_VENUSGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("c6e0a799-3471-4752-a73f-3981fdc31336");

        string IGameProviderConfiguration.Name => "AWC_VENUS-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_VENUS-GPI";

        string IGameProviderConfiguration.SecretKey => "waUMzINTTiZMsNmTxZnPfOdeeZCdvKRI";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpivenus",
            cert = "DfyfVhwJ8akfz2aGo3K",
            betLimit = new
            {
                VENUS = new
                {
                    LIVE = new
                    {

                        limitId = new int[] {
                            280303,
                            280304,
                            280305
                        }
                    }
                }
            }
        });
    }
}
