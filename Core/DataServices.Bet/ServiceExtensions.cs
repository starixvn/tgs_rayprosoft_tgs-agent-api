using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices.Bet
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBetRepository(this IServiceCollection services,
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
                option.EnableSensitiveDataLogging();
#endif
                option.UseSqlServer(connectionString, serverOption =>
                {
                    serverOption.CommandTimeout(180);
                    serverOption.EnableRetryOnFailure(3);
                    serverOption.MigrationsHistoryTable("_MigrationsHistory", "dbo");
                });

                using var db = new BetRepository((DbContextOptions<BetRepository>)option.Options);

                var isMigrate = !db.Database.GetService<IRelationalDatabaseCreator>().Exists() ||
                                db.Database.GetPendingMigrations().Any();

                if (isMigrate)
                {
                    db.Database.Migrate();
                }
            }, 128);

            return services;
        }
    }
}
