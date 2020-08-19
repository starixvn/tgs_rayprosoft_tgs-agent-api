using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TGS.Services.Operations.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepository<TInterface, TRepository>(this IServiceCollection services,
            string connectionString, int commandTimeout = 180, int maxRetryCount = 3)
            where TInterface : class
            where TRepository : DbContext, TInterface
        {
            services.AddDbContextPool<TRepository>((option) =>
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
                    serverOption.CommandTimeout(commandTimeout);
                    serverOption.EnableRetryOnFailure(maxRetryCount);
                });
            }, 128);

            AddDbMigration<TRepository>(connectionString);
            services.AddScoped<TInterface, TRepository>();

            return services;
        }

        private static void AddDbMigration<TRepository>(string connectionString) where TRepository : DbContext
        {
            var options = CreateBetDbOptionsBuilder<TRepository>(connectionString).Options;
            using var db = (DbContext) Activator.CreateInstance(typeof(TRepository), options);
            if (db == null ||
                db.Database.GetService<IRelationalDatabaseCreator>().Exists() ||
                !db.Database.GetPendingMigrations().Any())
            {
                return;
            }
            
            db.Database.Migrate();
        }

        private static DbContextOptionsBuilder<TRepository>
            CreateBetDbOptionsBuilder<TRepository>(string connectionString) where TRepository : DbContext
        {
            return new DbContextOptionsBuilder<TRepository>()
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
