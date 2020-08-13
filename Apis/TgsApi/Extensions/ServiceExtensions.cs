using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices;

namespace TGS.TgsApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogDbContext(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContextPool<TgsRepository>((option) =>
            {
                //#if DEBUG
                option.UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                }));
                option.EnableSensitiveDataLogging(true);
                //#endif
                option.UseSqlServer(connectionString, serverOption =>
                {
                    serverOption.CommandTimeout(180);
                    serverOption.EnableRetryOnFailure(3);
                    serverOption.MaxBatchSize(2000);
                });
            }, 128);

            return services;
        }
    }
}
