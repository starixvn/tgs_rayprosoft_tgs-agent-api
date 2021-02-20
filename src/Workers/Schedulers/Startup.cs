using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using TGS.Core.DataServices.Report.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.Hangfire;
using TGS.Core.Infrastructures.Hangfire.Filters;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Workers.Schedulers.Jobs;
using TGS.Workers.Schedulers.Jobs.Reports;
using TGS.Workers.Schedulers.Services;
using GlobalConsts = TGS.Core.Models.Consts;

namespace TGS.Workers.Schedulers
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs")
                .AddReportRepository(_configuration, "Report")
                .AddWebApiServices()
                .AddNats(_configuration)
                .AddTgsLog()
                .AddJson()
                .AddRedis()
                .AddTransactions()
                .AddHangfireScheduler(_configuration, jobProvider: new JobProvider(), types: GetModuleTypes());

            services.AddControllers();

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddRedis(_configuration)
                .AddMongoDb(_configuration, "Hangfire", "hangfire-mongodb")
                .ForwardToPrometheus();

            // add custom services for workers
            services.AddTransient<IBrandGameProviderService, BrandGameProviderService>();
            services.AddTransient<IMarketService, MarketService>();
            services.AddTransient<IMarketReportService, MarketReportService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseHangfireScheduler(new[] {Consts.Queue.Schedule});

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResultStatusCodes = GlobalConsts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                });

                endpoints.MapMetrics();

                endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] {new IgnoreHangfireAuthorizationFilter()}
                });
            });
        }

        private static Type[] GetModuleTypes()
        {
            return new[] {typeof(JobManagement)};
        }
    }
}