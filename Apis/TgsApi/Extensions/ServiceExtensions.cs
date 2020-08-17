using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;

namespace TGS.Apis.TgsApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            string tgsConnectionString,
            string betConnectionString)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddEFSecondLevelCache();

            services.AddBetRepository(betConnectionString);
            AddBetDbMigration(betConnectionString);
            services.AddScoped<IBetRepository, BetRepository>();

            services.AddTgsRepository(tgsConnectionString);
            AddTgsDbMigration(tgsConnectionString);
            services.AddScoped<ITgsRepository, TgsRepository>();

            return services;
        }

        private static void AddTgsDbMigration(string connectionString)
        {
            using var db = new TgsRepository(CreateTgsDbOptionsBuilder(connectionString).Options);
            if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                db.Database.Migrate();
            }
            else if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }
        }

        private static DbContextOptionsBuilder<TgsRepository> CreateTgsDbOptionsBuilder(string connectionString)
        {
            return new DbContextOptionsBuilder<TgsRepository>()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(connectionString, serverOptions =>
                {
                    serverOptions.CommandTimeout(5 * 60);
                    serverOptions.EnableRetryOnFailure(3);
                    serverOptions.MigrationsHistoryTable("_MigrationsHistory", "dbo");
                });
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
