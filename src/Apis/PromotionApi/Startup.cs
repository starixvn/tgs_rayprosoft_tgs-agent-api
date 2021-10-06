using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using TGS.Apis.PromotionApi.Middlewares;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Promotion.MessageQueue;

namespace TGS.Apis.PromotionApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var defaultBypassRouting = new[] { "/health", "/metrics", "/favicon.ico", "/swagger" };
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Promotion API"));
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs");

            services.AddNats(_configuration)
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddApiClient()
                .AddPlayerBucketRepositoriesServices();

            services
                .AddPromotionCore()
                .ConfigureWallet(config =>
                {
                    config
                        .AddTgsWalletRepositories()
                        .AddTgsAgentWalletRepositories();
                })
                .AddTgsPromotionRepositories()
                .AddPaymentServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = {
                            new JsonStringEnumConverter()
                        }
                    };
                    options.PaymentServiceBaseUrl = config.GetValue<Uri>("Promotion:PaymentServiceBaseUrl");
                });

            services.AddFileServiceClient((sp, httpClient) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                httpClient.BaseAddress = new(config.GetValue<string>("Promotion:FileServiceBaseUrl"));
            });

            services.AddNatsMessageQueue(config =>
            {
                config
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();

                        options.Url = configuration.GetValue<string>("NatsSetting:Urls");
                    })
                    .AddHandler<DepositStatusChangedMessageHandler>(
                        "tgs.payment.deposit.status",
                        "tgs.payment.deposit.status")
                    .ProcessAllSubjects();
            });

            services
                .AddHttpContextAccessor()
                .AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TGS Promotion API", Version = "v1" });

                var xml = "TGS.Apis.Promotion.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });
        }
    }
}
