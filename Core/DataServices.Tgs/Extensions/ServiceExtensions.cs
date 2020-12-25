using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsRepository(this IServiceCollection services, IConfiguration configuration, string connectionName)
        { 
            services.AddRepository<ITgsRepository, TgsRepository>(configuration, connectionName);

            return services;
        }
    }
}
