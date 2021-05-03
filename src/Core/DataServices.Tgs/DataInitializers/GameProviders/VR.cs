using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record VRGameProvider : IGameProvider
        {
            private static readonly string EndpointPath = "?playerName={PLAYERNAME}&loginTime={LOGINTIME}&channelId={CHANNELID}&departureUrl={DEPARTUREURL}&isCreateUser={ISCREATEUSER}&channelType={CHANNELTYPE}";
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public VRGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.VR)
                    .Select(g =>
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                        {
                            return g;
                        }

                        return (Game)g with { EndpointPath = EndpointPath };
                    })
                    .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("AE5533BD-35D5-5478-19D6-233ED44F7C9D");

            string IGameProvider.Code => GameProviderConst.VR;

            string IGameProvider.Name => "VR";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[] { new VRGameProviderConfiguration() };

            ICurrency[] IGameProvider.Currencies => new ICurrency[] { };

            ILanguage[] IGameProvider.Languages => new ILanguage[] { };

            IGame[] IGameProvider.Games => _games;

            short IGameProvider.UniqueId => 11;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new VRConfiguration
            {
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-VR") : new("http://localhost:62330"),
                ApiUrl = new("https://api.prerelease-env.biz"),
                Deskey = "PLFNNVZ68DF4V88XZ8B0688660Z2LBP2"
            };
        }

        internal class VRGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _Endpoint = "https://tgs.prerelease-env.biz/gs2c/playGame.do";

            Guid IGameProviderConfiguration.Id => new("FE4DDD30-9E2B-630F-9C0F-9E8D80C00B3B");

            string IGameProviderConfiguration.Name => "Configuration VR";

            string IGameProviderConfiguration.ClientId => "VR-WG";

            string IGameProviderConfiguration.SecretKey => "ewe7jRlkXKwcGLaPOyculQIObOuqPxy7";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}
