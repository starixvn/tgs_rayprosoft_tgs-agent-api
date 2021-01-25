using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Bet.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBetRepository(this IServiceCollection services,
            IConfiguration configuration, string connectionName)
        {
            services.AddRepository<IBetRepository, BetRepository>(configuration, connectionName);
            return services;
        }
    }
}