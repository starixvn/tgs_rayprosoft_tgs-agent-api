using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Report.Migrations
{
    public class ReportContextFactory : IDesignTimeDbContextFactory<ReportRepository>
    {
        public ReportRepository CreateDbContext(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .AddJsonFile("appsettings.json");
            var config = configBuilder.Build();

            var services = new ServiceCollection();

            services.AddRepository<ReportRepository, ReportRepository>(config, "Report");

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            services.AddSingleton(loggerFactory);

            var serviceProvider = services.BuildServiceProvider();

            var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ReportRepository>>();

            return dbContextFactory.CreateDbContext();
        }
    }
}