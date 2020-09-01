using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices.Base.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepository<TInterface, TRepository>(this IServiceCollection services,
            IConfiguration configuration, string connectionName, int commandTimeout = 180, int maxRetryCount = 3)
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

                var connectionString = configuration.GetConnectionString(connectionName);

                option.UseSqlServer(connectionString, serverOption =>
                {
                    serverOption.CommandTimeout(commandTimeout);
                    serverOption.EnableRetryOnFailure(maxRetryCount);
                    serverOption.MigrationsHistoryTable("_MigrationsHistory", "dbo");
                });

                var options = (DbContextOptions<TRepository>)option.Options;
                using var db = (DbContext)Activator.CreateInstance(typeof(TRepository), options);

                var isMigrate = !db.Database.GetService<IRelationalDatabaseCreator>().Exists() ||
                                db.Database.GetPendingMigrations().Any();

                if (isMigrate)
                {
                    db.Database.Migrate();
                }
            }, 128);

            services.AddScoped<TInterface, TRepository>();
            services.AddTransient<Func<TInterface>>(c => () => c.GetService<TInterface>());

            return services;
        }
    }
}
