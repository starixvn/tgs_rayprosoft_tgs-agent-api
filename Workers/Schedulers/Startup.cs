using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
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
                .AddWebApiServices()
                .AddNats(_configuration)
                .AddTgsLog()
                .AddJson()
                .AddRedis()
                .AddTransactions()
                .AddHangfireScheduler(_configuration, jobProvider: new JobProvider());

            services.AddControllers();

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddRedis(_configuration)
                .AddMongoDb(_configuration, "Hangfire", "hangfire-mongodb")
                .ForwardToPrometheus();

            // add custom services for workers
            services.AddTransient<IBrandGameProviderService, BrandGameProviderService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseHangfireScheduler(new[] {Consts.Queue.Schedule});

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = GlobalConsts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                });

                endpoints.MapMetrics();

                endpoints.MapHangfireDashboard("/hangfire", new()
                {
                    Authorization = new[] {new IgnoreHangfireAuthorizationFilter()}
                });
            });
        }
    }
}