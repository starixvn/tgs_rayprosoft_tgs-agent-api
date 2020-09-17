using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Bet;

namespace TGS.Apis.TgsApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRepository<IBetRepository, BetRepository>(configuration, "Bet");
            services.AddRepository<ITgsRepository, TgsRepository>(configuration, "Tgs");
            return services;
        }
    }
}
