using System;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record BetConstructProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public BetConstructProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.BetConstruct)
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

        Guid IGameProvider.Id => new("39247048-E631-0AAA-4553-865BB86D27EC");

        string IGameProvider.Code => GameProviderConst.BetConstruct;

        string IGameProvider.Name => "BetConstruct";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
            new IGameProviderConfiguration[]
            {
                new BetConstructGameProviderConfiguration()
            };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency(),
            new THBCurrency(),
            new MYRCurrency(),
            new VNDCurrency(),
            new INRCurrency(),
            new IDRCurrency(),
            new HKDCurrency(),
            new PHPCurrency(),
            new EURCurrency(),
            new GBPCurrency(),
            new KRWCurrency(),
            new USDCurrency()
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en"),
            new CNLanguage("zh"),
            new TWLanguage("zh")
        };

        IGame[] IGameProvider.Games => _games;

        bool IGameProvider.AllowKickPlayer => false;

        bool IGameProvider.TransferWallet => false;

        ushort IGameProvider.UniqueId => 21;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new SubGameProvider(GameProviderConst.BetConstruct,"BetConstruct")
        };

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new BetConstructConfiguration
            {
                ServiceEndpointUrl = new Uri("http://localhost:62330"),
                SharedKey = "jiLEPYw5n3n5kcx6T3",
                PartnerId = "1877685"
            };
    }

    internal class BetConstructGameProviderConfiguration : IGameProviderConfiguration
    {
        string IGameProviderConfiguration.ClientId => "BetConstruct-PG";

        string IGameProviderConfiguration.DesktopEndpoint => "https://devsportsbook.srvttt.com";

        Guid IGameProviderConfiguration.Id => new("575A70EC-1004-B324-4444-2ADE0261A0B2");

        string IGameProviderConfiguration.MobileEndpoint => "https://devmobile.srvttt.com";

        string IGameProviderConfiguration.Name => "Configuration BetConstruct";

        string IGameProviderConfiguration.SecretKey => "jiLEPYw5n3n5kcx6T3";
    }
}
