using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices.Base.Extensions;

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

            var services = new ServiceCollection();

            services.AddRepository<TgsRepository, TgsRepository>(config, "Tgs");

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            services.AddSingleton<ILoggerFactory>(loggerFactory);

            var serviceProvider = services.BuildServiceProvider();

            var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<TgsRepository>>();

            return dbContextFactory.CreateDbContext();
        }
    }
}
