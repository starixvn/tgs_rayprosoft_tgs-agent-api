using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices.Bet
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsRepository(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContextPool<BetRepository>((option) =>
            {
#if DEBUG
                option.UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                }));
                option.EnableSensitiveDataLogging(true);
#endif
                option.UseSqlServer(connectionString, serverOption =>
                {
                    serverOption.CommandTimeout(180);
                    serverOption.EnableRetryOnFailure(3);
                });
            }, 128);
            return services;
        }
    }
}
