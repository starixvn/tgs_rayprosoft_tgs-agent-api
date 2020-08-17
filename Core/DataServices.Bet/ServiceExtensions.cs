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
                });
            }, 128);

            AddBetDbMigration(connectionString);
            services.AddScoped<IBetRepository, BetRepository>();

            return services;
        }

        private static void AddBetDbMigration(string connectionString)
        {
            using var db = new BetRepository(CreateBetDbOptionsBuilder(connectionString).Options);
            if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                db.Database.Migrate();
            }
            else if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }
        }

        private static DbContextOptionsBuilder<BetRepository> CreateBetDbOptionsBuilder(string connectionString)
        {
            return new DbContextOptionsBuilder<BetRepository>()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(connectionString, serverOptions =>
                {
                    serverOptions.CommandTimeout(5 * 60);
                    serverOptions.EnableRetryOnFailure(3);
                    serverOptions.MigrationsHistoryTable("_MigrationsHistory", "dbo");
                });
        }
    }
}
