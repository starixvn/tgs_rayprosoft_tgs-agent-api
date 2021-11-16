using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Infrastructures.Extensions;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record BetConstructProvider : IGameProvider
        {            
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public BetConstructProvider(IWebHostEnvironment env)
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

                _env = env;
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
                new EURCurrency(),
                new VNDCurrency(),
                new GBPCurrency(),
                new KRWCurrency(),
                new USDCurrency(),
                new VNDCurrency()//TODO:須詢問廠商是否為1:1000的VND
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

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new BetConstructConfiguration
            {
                ServiceEndpointUrl =
                    _env.IsDocker() ? new Uri("http://game-betconstruct") : new("http://localhost:62330")
            };
        }
        internal class BetConstructGameProviderConfiguration : IGameProviderConfiguration
        {
            private static readonly string _endpoint = string.Empty;
            Guid IGameProviderConfiguration.Id => new("575A70EC-1004-B324-4444-2ADE0261A0B2");

            string IGameProviderConfiguration.Name => "Configuration BetConstruct";

            string IGameProviderConfiguration.ClientId => "BetConstruct-PG";

            string IGameProviderConfiguration.SecretKey => "GaQMVGBCScM8JPJYjhcrxj5VDDT2QqAa";

            string IGameProviderConfiguration.DesktopEndpoint => _endpoint;

            string IGameProviderConfiguration.MobileEndpoint => _endpoint;
        }
    }
}
