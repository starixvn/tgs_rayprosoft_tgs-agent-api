using Microsoft.Extensions.DependencyInjection;
using TGS.Shared.Game.ApiClients.AsiaGaming;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.PragmaticPlay;
using TGS.Shared.Game.ApiClients.SBOBET;

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
            services.AddHttpClient<IAsiaGamingApiClient, AsiaGamingApiClient>();

            return services;
        }
    }
}