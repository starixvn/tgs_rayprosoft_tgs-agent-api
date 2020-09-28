using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Json
{
    /// <summary>
    /// Dependency Injection of Infrastructures.Json
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddJson(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();
            return services;
        }
    }
}
