using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public class TgsContextFactory : IDesignTimeDbContextFactory<TgsRepository>
    {
        public TgsRepository CreateDbContext(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var optionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            optionsBuilder
                .UseNpgsql(config.GetConnectionString("Tgs"))
                .UseLoggerFactory(loggerFactory);

            return new TgsRepository(optionsBuilder.Options);
        }
    }
}
