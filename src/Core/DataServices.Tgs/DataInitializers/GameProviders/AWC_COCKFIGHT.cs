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
    internal record AWC_COCKFIGHTGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_COCKFIGHTGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_COCKFIGHT)
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

        Guid IGameProvider.Id => new("24EA3C40-65F3-427B-8DC8-583075AEBFB5");

        string IGameProvider.Code => GameProviderConst.AWC_COCKFIGHT;

        string IGameProvider.Name => "AWC_COCKFIGHT";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_COCKFIGHTGameProviderConfiguration()
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

        ushort IGameProvider.UniqueId => 37;

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
            new(GameProviderConst.AWC_COCKFIGHT, GameProviderConst.AWC_COCKFIGHT)
        };
    }

    internal record AWC_COCKFIGHTGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("BE30EED9-DA11-4FC1-BD4A-12C39CDD8343");

        string IGameProviderConfiguration.Name => "AWC_COCKFIGHT-GPI";

        string IGameProviderConfiguration.ClientId => "AWC_COCKFIGHT-GPI";

        string IGameProviderConfiguration.SecretKey => "xkagvlwagrdclklidtmwkpmpmhhhqzug";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "cnygpicockfight",
            cert = "qsXFZmTNeLiMrNz1cIJ",
            betLimit = new
            {
                SV388 = new
                {
                    LIVE = new
                    {
                        maxbet = 2400,
                        minbet = 1,
                        mindraw = 1,
                        matchlimit = 4800,
                        maxdraw = 1200
                    }
                }
            }
        });
    }
}
