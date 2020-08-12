using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TGS.Apis.LogApi.DataService
{
    public sealed class DBContextFactory : IDesignTimeDbContextFactory<LogDbContext>
    {
        public LogDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("LogDB");

            var dbContextBuilder = new DbContextOptionsBuilder<LogDbContext>()
                 .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                 .UseSqlServer(connectionString, serverOptions =>
                 {
                     serverOptions.CommandTimeout(5 * 60);
                     serverOptions.EnableRetryOnFailure(3);
                     serverOptions.MigrationsHistoryTable("_EFCoreMigrationHistory", "dbo");
                 });

            return new LogDbContext(dbContextBuilder.Options);
        }
    }
}
