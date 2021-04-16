using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TGS.Core.Models;

namespace TGS.Core.DataServices.Base.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepository<TInterface, TRepository>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionName,
            int commandTimeout = 180)
            where TInterface : class
            where TRepository : DbContext, TInterface
        {
            const int poolSize = 64;
            var connectionString = configuration.GetConnectionString(connectionName);

            services.AddDbContextPool<TRepository>(option =>
            {
                AddLogger(option);

                option.UseLazyLoadingProxies();

                option.UseNpgsql(connectionString, serverOption =>
                {
                    serverOption.SetPostgresVersion(12, 5);
                    serverOption.CommandTimeout(commandTimeout);
                    serverOption.MigrationsHistoryTable(Consts.Db.MigrationsHistoryTable, Consts.Db.Schema);
                    serverOption.UseTrigrams();
                });

                option.UseSnakeCaseNamingConvention();


            }, poolSize);

            services.AddDbContextFactory<TRepository>(option =>
            {
                AddLogger(option);

                option
                    .UseNpgsql(connectionString, serverOption =>
                    {
                        serverOption.SetPostgresVersion(12, 5);
                        serverOption.CommandTimeout(commandTimeout);
                        serverOption.MigrationsHistoryTable(Consts.Db.MigrationsHistoryTable, Consts.Db.Schema);
                        serverOption.UseTrigrams();
                    })
                    .UseSnakeCaseNamingConvention();
            });

            services.AddScoped<TInterface>(c => c.GetRequiredService<TRepository>());
            services.AddTransient<Func<TInterface>>(c => () =>
            {
                var factory = c.GetRequiredService<IDbContextFactory<TRepository>>();
                return factory.CreateDbContext();
            });

            return services;
        }

        private static void AddLogger(DbContextOptionsBuilder option)
        {
#if DEBUG
            option.EnableSensitiveDataLogging();
#endif
        }
    }
}