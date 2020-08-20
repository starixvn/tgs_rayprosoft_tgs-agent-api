using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositoriesAndExecuteMigrations(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRepository<IBetRepository, BetRepository>(configuration, "BET");
            services.AddRepository<ITgsRepository, TgsRepository>(configuration, "TGS");
            return services;
        }
    }
}
