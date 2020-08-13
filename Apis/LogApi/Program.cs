using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.Extensions;

namespace TGS.Apis.LogApi
{
    public class Program
    {
        private static string _connectionString;
        private static IHost _host;
        public static void Main(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
            ProcessDbMigration();
            _host.Run();


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    _connectionString = hostContext.Configuration.GetConnectionString("LogDb");
                    services.AddLogDbContext(_connectionString);
                })
        ;

        private static void ProcessDbMigration()
        {
            using var db = new LogDbContext(CreateDbOptionsBuilder().Options);
            if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                db.Database.Migrate();
            }
        }

        private static DbContextOptionsBuilder<LogDbContext> CreateDbOptionsBuilder()
        {
            return new DbContextOptionsBuilder<LogDbContext>()
                .UseLoggerFactory(LoggerFactory.Create((builder) => builder.AddConsole()))
                .UseSqlServer(_connectionString, serverOptions =>
                {
                    serverOptions.CommandTimeout(180);
                    serverOptions.MigrationsHistoryTable("_MigrationsHistory", "dbo");
                });
        }
    }
}
