using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Extensions;

namespace TGS.Apis.TgsApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services,
            IConfiguration configuration)
        {            
            services.AddTgsRepository(configuration, "Tgs");
            services.AddBetRepository(configuration, "Bet");

            return services;
        }
    }
}
