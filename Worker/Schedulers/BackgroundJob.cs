using System;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;

namespace TGS.Worker.Schedulers
{
    public static class BackgroundJob
    {
        public static void Start(AppConfig config)
        {
            var mongoOptions = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new DropMongoMigrationStrategy(),
                    BackupStrategy = new NoneMongoBackupStrategy()
                },
                Prefix = "hangfire.mongo"
            };

            var mongoStorage = new MongoStorage(
                MongoClientSettings.FromConnectionString(config.Hangfire.ConnectionString),
                config.Hangfire.DbName, // database name
                mongoOptions);


            var options = new BackgroundJobServerOptions
            {
                SchedulePollingInterval = TimeSpan.FromSeconds(config.Hangfire.Schedule.SchedulePollingInterval)
            };

            using var server = new BackgroundJobServer(options, mongoStorage);
            Console.WriteLine("Hangfire Server started. Press any key to exit...");
            Console.ReadKey();
        }
    }
}