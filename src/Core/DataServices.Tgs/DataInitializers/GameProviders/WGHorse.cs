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
        internal record WGHorseGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public WGHorseGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.WGHorse)
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

            Guid IGameProvider.Id => new("AF7816C3-04DD-85ED-9826-758C4C833917");

            string IGameProvider.Code => GameProviderConst.WGHorse;

            string IGameProvider.Name => "WG Horse";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new WGHorseGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new CNYCurrency("RMB"),new HKDCurrency("HKD")
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new CNLanguage("0"),new TWLanguage("1"),new USLanguage("2")
            };

            IGame[] IGameProvider.Games => _games;

            ushort IGameProvider.UniqueId => 16;

            bool IGameProvider.AllowKickPlayer => false;

            bool IGameProvider.SupportTestPlayer => false;

            bool IGameProvider.TransferWallet => true;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new WGHorseConfiguration
            {
                AmountMaximum = ((decimal)Math.Pow(10, 12)) - 1,
                AmountFormat = "12,2",
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-wgvia") : new("http://localhost:62330"),
                ApiUrl = new("http://61.56.84.139/via6/index.php"),
                PartnerCode = "PT",
                PartnerKey = "a888888888888888",
                IV = "b888888888888888"
            };

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new(GameProviderConst.WGHorse, GameProviderConst.WGHorse)
            };
        }

        internal class WGHorseGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _Endpoint = string.Empty;

            Guid IGameProviderConfiguration.Id => new("C6B4B853-6BAA-80E3-90E8-9D52AAF7E960");

            string IGameProviderConfiguration.Name => "Configuration WG Horse";

            string IGameProviderConfiguration.ClientId => "WGVIA-PG";

            string IGameProviderConfiguration.SecretKey => "vomgxnxothbvsbrgzchvsrrssochqjzx";

            string IGameProviderConfiguration.DesktopEndpoint => _Endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _Endpoint;
        }
    }
}