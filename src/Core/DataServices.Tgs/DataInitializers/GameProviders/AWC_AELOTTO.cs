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
    internal record AWC_AELOTTOGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public AWC_AELOTTOGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.AWC_AELOTTO)
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

        Guid IGameProvider.Id => new("B0F5D350-DCAB-411A-A262-FB099FABEBDF");

        string IGameProvider.Code => GameProviderConst.AWC_AELOTTO;

        string IGameProvider.Name => "AWC_AELOTTO";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations => new IGameProviderConfiguration[]
        {
            new AWC_AELOTTOGameProviderConfiguration()
        };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new THBCurrency("THB")
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

        ushort IGameProvider.UniqueId => 29;

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
            new(GameProviderConst.AWC_AELOTTO, GameProviderConst.AWC_AELOTTO),
            new("AWC_YEEKEE_LOTTO", "AWC_YEEKEE_LOTTO")
        };
    }

    internal record AWC_AELOTTOGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _endpoint = string.Empty;

        Guid IGameProviderConfiguration.Id => new("ED5E5E92-959C-7E5A-024A-17E6C379C518");

        string IGameProviderConfiguration.Name => "AWC_AELOTTO-GPI_THB";

        string IGameProviderConfiguration.ClientId => "AWC_AELOTTO-GPI_THB";

        string IGameProviderConfiguration.SecretKey => "algpidxzgdfsafvtnthehtmymmqjmquj";

        string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

        string IGameProviderConfiguration.MobileEndpoint => _endpoint;

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            agentId = "thbgpiaelotto",
            cert = "sJQKnLeKugNHE7yqW7C"
        });
    }
}
