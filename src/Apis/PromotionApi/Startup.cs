using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using TGS.Core.DataServices.Report.Extensions;
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
using Tgs.Payment.LeePay;
using TGS.Services.BackOffice.Application.Extensions;
using TGS.Services.BackOffice.Domain;
using TGS.Services.Operations.Extensions;
using TGS.Services.PluggableServices.Extensions;
using TGS.Services.Report.Application.Extensions;

namespace PromotionApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
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
                .AddErrorMappers()
                .AddApiClient()
                .AddPluggableServices()
                .AddReportServices()
                .AddTransactions()
                .AddBackOfficeApplicationServices()
                .AddPromotionRepositoriesServices()
                .AddPlayerBucketRepositoriesServices()
                .AddBackendOfficeRepositories();

            services.AddWallet().AddTgsWalletRepositories().AddTgsAgentWalletRepositories();
            services
                .AddPayment()
                .UseTgsPaymentRepository()
                .RegisterWithdrawProcessor<LeePayProcessor>("LeePay");

            services
                .AddPaymentClient()
                .AddSingleton<LeePaySerializer>()
                .AddNatsMessageQueue(config =>
                {
                    config.ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();

                        options.Url = configuration.GetValue<string>("NatsSetting:Urls");
                    });
                });

            services.AddHttpContextAccessor();
            services.AddControllers();

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Report", "report-db");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() {Title = "TGS Promotion API", Version = "v1"});

                c.CustomSchemaIds(type => type.FullName.Replace('.', '_'));

                var xml = "TGS.Apis.Promotion.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Promotion API"));
            }
        }
    }
}