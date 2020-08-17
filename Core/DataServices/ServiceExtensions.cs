using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsRepository(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContextPool<TgsRepository>((option) =>
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

            AddTgsDbMigration(connectionString);
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
    }
}
