// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record PragmaticPlayGameProvider : IGameProvider
    {
        private static readonly string EndpointPath = "?key={KEY}&stylename={SECURELOGIN}";
        private readonly IGame[] _games;

        public PragmaticPlayGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.PragmaticPlay)
                .Select(g =>
                {
                    if (g.EndpointPath.IsNotNullOrEmpty())
                    {
                        return g;
                    }

                    return (Game)g with
                    {
                        EndpointPath = EndpointPath
                    };
                })
                .ToArray();
        }

        Guid IGameProvider.Id => new("5DACA63E-4265-4791-ADDA-CF6E935D39E0");

        string IGameProvider.Code => GameProviderConst.PragmaticPlay;

        string IGameProvider.Name => "PragmaticPlay";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
            new IGameProviderConfiguration[]
            {
                new PragmaticPlayGameProviderConfiguration()
            };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY"),
            new USDCurrency("USD"),
            new VNDCurrency("VND"),
            new IDRCurrency("IDR"),
            new VND1000Currency("VND2"),
            new IDR1000Currency("IDR2"),
            new JPYCurrency("JPY"),
            new EURCurrency("EUR"),
            new KRWCurrency("KRW"),
            new MYRCurrency("MYR"),
            new THBCurrency("THB"),
            new GBPCurrency("GBP")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new USLanguage("en"),
            new THLanguage("th"),
            new JPLanguage("ja"),
            new IDLanguage("id"),
            new VNLanguage("vi"),
            new CNLanguage("zh"),
            new TWLanguage("zt"),
            new KRLanguage("ko")
        };

        IGame[] IGameProvider.Games => _games;

        ushort IGameProvider.UniqueId => 3;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() => new PragmaticPlayConfiguration
        {
            ServiceEndpointUrl = new("http://localhost:62330"),
            SecureLogin = "tgs_tgs",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay",
            ApiUrl = new("https://api.prerelease-env.biz"),
            DelaySecondsForPlaceBet = 0
        };

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.PragmaticPlay, "PragmaticPlay")
        };
    }

    internal class PragmaticPlayGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _Endpoint = "https://tgs.prerelease-env.biz/gs2c/playGame.do";

        string IGameProviderConfiguration.ClientId => "PP-PG";

        string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

        Guid IGameProviderConfiguration.Id => new("455B0CA8-64B1-40A9-8C0A-FCFD3FE895FC");

        string IGameProviderConfiguration.MobileEndpoint => _Endpoint;

        string IGameProviderConfiguration.Name => "Configuration PragmaticPlay";

        string IGameProviderConfiguration.SecretKey => "1ofNpUR2B9UzkODEZCU2ugqBfk2mtdNu";
    }
}
