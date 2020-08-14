using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices
{
    class Program
    {
        private static IHost _host;
        private static ILogger _logger;
        private static string _connectionString;

        public static void Main(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
            _logger = _host.Services.GetRequiredService<ILogger<Program>>();

            _logger.LogInformation("Start DB Migration");
            ProcessDbMigration();
            _logger.LogInformation("DB Migration Done");
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                           .ConfigureHostConfiguration((config) =>
                           {
                               config.AddEnvironmentVariables();
                           })
                           .ConfigureAppConfiguration((hostContext, config) =>
                           {
                               config.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                               config.AddJsonFile("appsettings.json", false, true);
                               if (args != null)
                               {
                                   config.AddCommandLine(args);
                               }
                           })
                           .ConfigureServices((hostcontext, services) =>
                           {
                               services.AddEFSecondLevelCache();
                               _connectionString = hostcontext.Configuration.GetConnectionString("TGS");
                               services.AddTgsRepository(_connectionString);
                           })
                           .ConfigureLogging((hostcontext, loggin) =>
                           {
                               loggin.AddConfiguration(hostcontext.Configuration.GetSection("Logging"));
                               if (hostcontext.HostingEnvironment.IsDevelopment())
                               {
                                   loggin.AddConsole();
                                   loggin.SetMinimumLevel(LogLevel.Error);
                               }
                           });
        }

        private static void ProcessDbMigration()
        {
            using var db = new TgsRepository(CreateDbOptionsBuilder().Options);
            if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                db.Database.Migrate();
            }
        }

        private static DbContextOptionsBuilder<TgsRepository> CreateDbOptionsBuilder()
        {
            return new DbContextOptionsBuilder<TgsRepository>()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(_connectionString, serverOptions =>
                {
                    serverOptions.CommandTimeout(5 * 60);
                    serverOptions.EnableRetryOnFailure(3);
                    serverOptions.MigrationsHistoryTable("_EFCoreMigrationHistory", "dbo");
                });
        }
    }
}
