using Bonus.Extensions;
using Bonus.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using TGS.Core.DataServices.Report.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Operations.Extensions;
using TGS.Services.PluggableServices.Extensions;
using TGS.Services.Report.Application.Extensions;

namespace ReportApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReportRepository(_configuration, "Report");
            services.AddTgsRepository(_configuration, "Tgs");

            services.AddNats(_configuration)
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddOperations()
                .AddApiClient()
                .AddPluggableServices()
                .AddReportServices()
                .AddTransactions()
                .AddBonusServices()
                .AddBonusRepositoriesServices()
                .AddBonusRepositoriesServices();

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Report", "report-db");

            services.AddHttpContextAccessor();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGlobalExceptionHandler();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, env)
                });

                endpoints.MapMetrics();
            });
        }
    }
}