using EFSecondLevelCache.Core;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;

namespace TGS.Apis.TgsApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            string tgsConnectionString,
            string betConnectionString)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddEFSecondLevelCache();
            services.AddBetRepository(betConnectionString);
            services.AddTgsRepository(tgsConnectionString);
            return services;
        }
    }
}
