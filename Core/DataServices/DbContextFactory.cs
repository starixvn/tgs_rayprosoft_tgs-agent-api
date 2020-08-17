using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices
{
    public sealed class DbContextFactory : IDesignTimeDbContextFactory<TgsRepository>
    {
        public TgsRepository CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("TGS");

            var dbContextBuilder = new DbContextOptionsBuilder<TgsRepository>()
                .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()))
                .UseSqlServer(connectionString, serverOptions =>
                {
                    serverOptions.CommandTimeout(5 * 60);
                    serverOptions.EnableRetryOnFailure(3);
                    serverOptions.MigrationsHistoryTable("_MigrationHistory", "dbo");
                });

            return new TgsRepository(dbContextBuilder.Options);
        }
    }
}