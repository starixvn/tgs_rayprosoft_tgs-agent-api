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
            services.AddBetRepository(betConnectionString);
            services.AddTgsRepository(tgsConnectionString);
            services.AddScoped<ITgsRepository, TgsRepository>();
            services.AddScoped<IBetRepository, BetRepository>();
            return services;
        }
    }
}
