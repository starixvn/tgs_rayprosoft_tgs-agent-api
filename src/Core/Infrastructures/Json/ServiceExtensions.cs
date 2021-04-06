using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Json
{
    /// <summary>
    /// Dependency Injection of Infrastructures.Json
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSystemJsonSerializer(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();
            return services;
        }
    }
}
