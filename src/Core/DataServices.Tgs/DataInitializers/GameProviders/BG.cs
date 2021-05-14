// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

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
        internal record BGGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public BGGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.BG)
                    .Select(g =>
                    {
                        if (g.EndpointPath.IsNotNullOrEmpty())
                        {
                            return g;
                        }

                        return (Game)g with { EndpointPath = string.Empty };
                    })
                    .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("D76D09B7-D460-F6DA-E326-A4E9725A8080");

            string IGameProvider.Code => GameProviderConst.BG;

            string IGameProvider.Name => "BG";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[] { new BGGameProviderConfiguration() };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new MYRCurrency(), new THBCurrency(), new VNDCurrency(),
                new VND1000Currency()
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new USLanguage("en_US"), new CNLanguage("zh_CN"), new TWLanguage("zh_TW"),
                new THLanguage("th_TH"), new IDLanguage("id_ID"), new KRLanguage("ko_KR"),
                new JPLanguage("ja_JP"), new VNLanguage("vi_VN")
            };

            IGame[] IGameProvider.Games => _games;

            short IGameProvider.UniqueId => 8;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new BGConfiguration
            {
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-bg") : new("http://localhost:62330"),
                ApiUrl = new("https://api.prerelease-env.biz"),
                SecretCode = "tgs_tgs",
                SecretKey = "testKey"
            };
        }

        internal class BGGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("A9E3B270-79F5-B455-F476-679D693C82E7");

            string IGameProviderConfiguration.Name => "Configuration WG";

            string IGameProviderConfiguration.ClientId => "BG-WG";

            string IGameProviderConfiguration.SecretKey => "puhtqyhzlipnacxvublifaoldrkwbpth";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}