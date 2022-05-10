// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.VR;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal record VRGameProvider : IGameProvider
    {
        private readonly IGame[] _games;

        public VRGameProvider()
        {
            _games = LoadGamesFromCsv(GameProviderConst.VR)
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

        Guid IGameProvider.Id => new("AE5533BD-35D5-5478-19D6-233ED44F7C9D");

        string IGameProvider.Code => GameProviderConst.VR;

        string IGameProvider.Name => "VR";

        IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
            new IGameProviderConfiguration[]
            {
                new VRGameProviderConfiguration()
            };

        ICurrency[] IGameProvider.Currencies => new ICurrency[]
        {
            new CNYCurrency("CNY")
        };

        ILanguage[] IGameProvider.Languages => new ILanguage[]
        {
            new CNLanguage("zh")
        };

        IGame[] IGameProvider.Games => _games;

        ushort IGameProvider.UniqueId => 11;

        bool IGameProvider.AllowKickPlayer => false;

        bool IGameProvider.SupportTestPlayer => false;

        bool IGameProvider.TransferWallet => true;

        GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

        PluggableConfigurationBase IGameProvider.GetConfiguration() => new VRConfiguration
        {
            ServiceEndpointUrl = new Uri("http://localhost:62330"),
            AmountMaximum = 3000000m,
            AmountFormat = "10,3",
            BrandMerchants = new VRBrandMerchant[]
            {
                new()
                {
                    BrandCode = "Default",
                    ApiUrl = new("https://pmws.vrbetapi.com/"),
                    Version = "1.0",
                    Merchantid = "WGWG",
                    Deskey = "PLFNNVZ68DF4V88XZ8B0688660Z2LBP2",
                    Status = true
                }
            }
        };

        SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
        {
            new(GameProviderConst.VR, "VR")
        };
    }

    internal class VRGameProviderConfiguration : IGameProviderConfiguration
    {
        private static readonly string _Endpoint = "https://tgs.prerelease-env.biz/gs2c/playGame.do";

        string IGameProviderConfiguration.ClientId => "VR-PG";

        string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

        Guid IGameProviderConfiguration.Id => new("FE4DDD30-9E2B-630F-9C0F-9E8D80C00B3B");

        string IGameProviderConfiguration.MobileEndpoint => _Endpoint;

        string IGameProviderConfiguration.Name => "Configuration VR";

        string IGameProviderConfiguration.SecretKey => "ewe7jRlkXKwcGLaPOyculQIObOuqPxy7";
    }
}
