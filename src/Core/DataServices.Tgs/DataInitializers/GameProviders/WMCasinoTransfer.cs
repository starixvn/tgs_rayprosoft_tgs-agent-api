// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using System.Text.Json;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record WMCasinoTransferGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public WMCasinoTransferGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.WMCasinoTransfer)
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

        Guid IGameProvider.Id => new("0ed01cba-b2b4-4adb-b52d-77134e91abeb");

        string IGameProvider.Code => GameProviderConst.WMCasinoTransfer;

        string IGameProvider.Name => "WM Casino Transfer";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
            new IGameProviderConfiguration[]
            {
                new WMCasinoTransferGameProviderConfiguration()
            };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new CNLanguage("0"),
            new USLanguage("1"),
            new THLanguage("2"),
            new VNLanguage("3"),
            new JPLanguage("4"),
            new KRLanguage("5"),
            new IDLanguage("6"),
            new TWLanguage("9")
        };

        IGame[] IGameProvider.Games => _games;

        ushort IGameProvider.UniqueId => 42;

        bool IGameProvider.AllowKickPlayer => true;

        bool IGameProvider.SupportTestPlayer => true;

        bool IGameProvider.TransferWallet => true;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() =>
            new WMCasinoTransferConfiguration
            {
                ServiceEndpointUrl = new Uri("http://localhost:62330"),
                ApiUrl = new("https://api.a45.me/api/wallet/Gateway.php")
            };

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.WMCasinoTransfer, GameProviderConst.WMCasinoTransfer)
        };
    }

    internal class WMCasinoTransferGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _Endpoint = string.Empty;

        string IGameProviderConfiguration.ClientId => "WMTF-PG_GPI_CNY";

        string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

        Guid IGameProviderConfiguration.Id => new("d311b9b2-66c5-4f48-a32a-c72d77279596");

        string IGameProviderConfiguration.MobileEndpoint => _Endpoint;

        string IGameProviderConfiguration.Name => "WMTF-PG_GPI_CNY";

        string IGameProviderConfiguration.SecretKey => "XX3nixPCPay6Ekc";

        string IGameProviderConfiguration.ConfigurationData => JsonSerializer.Serialize(new
        {
            vendorId = "hpg02twapi",
            signature = "89448b444ed2e922ddc3aa0753370238"
        });
    }
}
