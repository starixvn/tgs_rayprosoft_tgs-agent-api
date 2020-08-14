using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices;

namespace TGS.TgsApi
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
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    _connectionString = hostContext.Configuration.GetConnectionString("TGS");
                    services.AddTgsRepository(_connectionString);
                })
        ;

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
                .UseLoggerFactory(LoggerFactory.Create((builder) => builder.AddConsole()))
                .UseSqlServer(_connectionString, serverOptions =>
                {
                    serverOptions.CommandTimeout(180);
                    serverOptions.MigrationsHistoryTable("_MigrationsHistory", "dbo");
                });
        }
    }
}
