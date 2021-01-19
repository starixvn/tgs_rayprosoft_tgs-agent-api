using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace TGS.Core.Infrastructures.Hangfire
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services,
            IConfiguration configuration, string configSectionName = "Hangfire")
        {
            services.Configure<HangfireSettings>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HangfireSettings>>().Value);

            services.AddHangfire((services, option) =>
                {
                    var settings = services.GetRequiredService<HangfireSettings>();
                    var mongoUrlBuilder = new MongoUrlBuilder(settings.ConnectionString);
                    var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

                    option.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSerializerSettings(
                            new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore})
                        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
                        {
                            MigrationOptions = new MongoMigrationOptions
                            {
                                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                BackupStrategy = new CollectionMongoBackupStrategy()
                            },
                            Prefix = settings.Prefix
                        });
                }
            );

            return services;
        }
    }
}