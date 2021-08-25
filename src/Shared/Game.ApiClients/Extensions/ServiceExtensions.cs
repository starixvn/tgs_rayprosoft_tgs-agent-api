using Microsoft.Extensions.DependencyInjection;
using TGS.Shared.Game.ApiClients.AllBet;
using TGS.Shared.Game.ApiClients.AsiaGaming;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.ApiClients.Evolution;
using TGS.Shared.Game.ApiClients.IMESports;
using TGS.Shared.Game.ApiClients.JDB;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.PragmaticPlay;
using TGS.Shared.Game.ApiClients.Saba;
using TGS.Shared.Game.ApiClients.SBOBET;
using TGS.Shared.Game.ApiClients.SexyGaming;
using TGS.Shared.Game.ApiClients.Skywind;
using TGS.Shared.Game.ApiClients.VR;
using TGS.Shared.Game.ApiClients.WGHorse;
using TGS.Shared.Game.ApiClients.WGSports;
using TGS.Shared.Game.ApiClients.WMCasino;

namespace TGS.Shared.Game.ApiClients.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        ///     Dependency Injection of Game.ApiClients, need to add following dependencies beforehand:
        ///     1. AddApiClient() in Infrastructures.ApiClient ( if BaseApiClient is used )
        /// </summary>
        public static IServiceCollection AddGameApiClients(this IServiceCollection services)
        {
            services.AddHttpClient<ISBOApiClient, SBOApiClient>();
            services.AddHttpClient<IPragmaticPlayApiClient, PragmaticPlayApiClient>();
            services.AddHttpClient<IKYApiClient, KYApiClient>();
            services.AddHttpClient<IJDBApiClient, JDBApiClient>();
            services.AddHttpClient<IAsiaGamingApiClient, AsiaGamingApiClient>();
            services.AddHttpClient<ISabaApiClient, SabaApiClient>();
            services.AddHttpClient<IVRApiClient, VRApiClient>();
            services.AddHttpClient<IIMESportsApiClient, IMESportsApiClient>();
            services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>();
            services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>();
            services.AddHttpClient<ISkywindApiClient, SkywindApiClient>();
            services.AddHttpClient<IAllBetApiClient, AllBetApiClient>();
            services.AddHttpClient<IEvolutionApiClient, EvolutionApiClient>();
            services.AddHttpClient<IWGSportsApiClient, WGSportsApiClient>();
            services.AddHttpClient<IWGHorseApiClient, WGHorseApiClient>();
            services.AddHttpClient<IWMCasinoApiClient, WMCasinoApiClient>();

            return services;
        }
    }
}