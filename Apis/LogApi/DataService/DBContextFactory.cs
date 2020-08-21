using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TGS.Apis.LogApi.Services;

namespace TGS.Apis.LogApi.DataService
{
    public sealed class DBContextFactory : IDesignTimeDbContextFactory<LogRepository>
    {
        public LogRepository CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("Log");

            var dbContextBuilder = new DbContextOptionsBuilder<LogRepository>()
                 .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                 .UseSqlServer(connectionString, serverOptions =>
                 {
                     serverOptions.CommandTimeout(5 * 60);
                     serverOptions.EnableRetryOnFailure(3);
                     serverOptions.MigrationsHistoryTable("_MigrationHistory", "dbo");
                 });

            return new LogRepository(dbContextBuilder.Options);
        }
    }
}
