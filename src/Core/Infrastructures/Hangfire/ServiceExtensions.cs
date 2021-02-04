using System;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using TGS.Core.Infrastructures.Hangfire.Services;

namespace TGS.Core.Infrastructures.Hangfire
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHangfireScheduler(this IServiceCollection services,
            IConfiguration configuration, string configSectionName = "Hangfire", BaseJobProvider jobProvider = null)
        {
            services.Configure<HangfireSettings>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HangfireSettings>>().Value);

            services.AddHangfire((sp, option) =>
            {
                var settings = sp.GetRequiredService<HangfireSettings>();
                var mongoUrlBuilder = new MongoUrlBuilder(settings.ConnectionString);
                var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

                option.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseColouredConsoleLogProvider()
                    .UseRecommendedSerializerSettings(s =>
                    {
                        s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        s.TypeNameHandling = TypeNameHandling.None;
                        s.NullValueHandling = NullValueHandling.Include;
                        s.DefaultValueHandling = DefaultValueHandling.Include;
                    })
                    .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new()
                    {
                        MigrationOptions = new()
                        {
                            MigrationStrategy = new DropMongoMigrationStrategy(),
                            BackupStrategy = new CollectionMongoBackupStrategy()
                        },
                        Prefix = settings.Prefix,
                        CheckConnection = false
                    });
            });

            if (jobProvider == null)
            {
                return services;
            }

            services.AddSingleton<IJobProvider>(jobProvider);
           
            var jobs = ((IJobProvider) jobProvider).GetJobDescriptions();
            foreach (var job in jobs)
            {
                services.AddTransient(job.Worker);
            }

            return services;
        }
    }
}