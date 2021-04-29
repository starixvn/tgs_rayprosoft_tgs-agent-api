using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Promotion.Client.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        ///     Dependency Injection of Operations, need to add the following dependencies beforehand:
        ///     1. AddOperations() in Infrastructures.Operations
        ///     2. AddApiClient() in Infrastructure.ApiClient
        /// </summary>
        public static IServiceCollection AddPromotionClientServices(this IServiceCollection services,
            IConfiguration configuration, string configSectionName = "Promotion")
        {
            services.Configure<PromotionSettings>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<PromotionSettings>>().Value);
            services.AddHttpClient<IPromotionClient, PromotionClient>((sp, c) =>
            {
                var settings = sp.GetRequiredService<PromotionSettings>();
                c.BaseAddress = new(settings.Url);
            });
            services.AddHttpClient<IPromotionBonusClient, PromotionBonusClient>((sp, c) =>
            {
                var settings = sp.GetRequiredService<PromotionSettings>();
                c.BaseAddress = new(settings.Url);
            });
            services.AddHttpClient<IPromotionDispatchClient, PromotionDispatchClient>((sp, c) =>
            {
                var settings = sp.GetRequiredService<PromotionSettings>();
                c.BaseAddress = new(settings.Url);
            });

            return services;
        }
    }
}