using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Infrastructures.Nats
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddNats(this IServiceCollection services, IConfiguration configuration, string configSectionName = "NatsSetting")
        {
            services.Configure<NatsSetting>(configuration.GetSection(configSectionName));
            services.AddScoped<IMessagePackProvider, MessagePackProvider>();
            services.AddScoped<INatsManager, NatsManager>();
            return services;
        }
    }
}