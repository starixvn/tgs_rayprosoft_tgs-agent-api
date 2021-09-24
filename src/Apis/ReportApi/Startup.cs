using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using TGS.Apis.ReportApi.Middlewares;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
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
using TGS.Services.Report.Extensions;

namespace TGS.Apis.ReportApi
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
            services.AddTgsRepository(_configuration, "Tgs");

            services
                .AddNats(_configuration)
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddOperations()
                .AddErrorMappers()
                .AddApiClient()
                .AddPluggableServices()
                .AddReportServices()
                .AddTransactions()
                .AddPlayerBucketRepositoriesServices();

            services
                .AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db");

            services.AddHttpContextAccessor();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "TGS Report API",
                    Version = "v1"
                });

                c.CustomSchemaIds(type => type.FullName.Replace('.', '_'));

                var xml = "TGS.Apis.ReportApi.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization(options =>
            {
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                options.SupportedCultures.Clear();
                options.SupportedUICultures.Clear();

                config.GetSection("RequestLocalization").Bind(options);
            });

            var defaultBypassRouting = new[]
            {
                "/health",
                "/metrics",
                "/favicon.ico",
                "/swagger"
            };
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseHttpMetrics();
            app.UseApiLogger(bypassRouting: defaultBypassRouting);

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

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Report API"));
            }
        }
    }
}