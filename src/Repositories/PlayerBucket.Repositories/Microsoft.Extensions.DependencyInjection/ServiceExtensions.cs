using PlayerBucket.Repositories;
using TGS.Services.PlayerBucket.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPlayerBucketRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IPlayerBucketRepository, PlayerBucketRepository>();

            return services;
        }
    }
}