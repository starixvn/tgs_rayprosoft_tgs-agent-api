using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using TGS.Apis.PromotionApi.Middlewares;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Promotion.MessageQueue;

namespace TGS.Apis.PromotionApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

                endpoints.MapHealthChecks("/health",
                    new HealthCheckOptions
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

            services
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddWebApiServices()
                .AddTgsLog()
                .AddApiClient()
                .AddPlayerBucketRepositoriesServices();

            services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            services
                .AddPromotionCore()
                .ConfigureWallet(config =>
                {
                    config
                        .AddTgsAgentWalletRepositories();
                })
                .AddTgsPromotionRepositories()
                .AddPaymentServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter() }
                    };
                    options.PaymentServiceBaseUrl = config.GetValue<Uri>("Promotion:PaymentServiceBaseUrl");
                })
                .AddCashServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Promotion:CashBaseUrl");
                });

            services.AddFileServiceClient((sp, httpClient) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                httpClient.BaseAddress = new Uri(config.GetValue<string>("Promotion:FileServiceBaseUrl"));
            });

            services
                .AddMessageQueue()
                .AddNatsMessageQueue(config =>
                {
                    config
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var configuration = sp.GetRequiredService<IConfiguration>();

                            options.Url = configuration.GetValue<string>("NatsSetting:Urls");
                        })
                        .AddHandler<DepositStatusChangedMessageHandler>(
                            "tgs.payment.deposit.status",
                            "tgs.payment.deposit.status");
                });

            services
                .AddHttpContextAccessor()
                .AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "TGS Promotion API",
                        Version = "v1"
                    });

                var xml = "TGS.Apis.Promotion.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);

                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });

            services
                .AddMessageQueue()
                .AddRabbitGlobPatternExchange("*", "amq.direct")
                .AddRabbitMessageQueue(config =>
                {
                    config
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var configuration = sp.GetRequiredService<IConfiguration>();

                            configuration.GetSection("RabbitMQ").Bind(options);
                        });
                });

            services
                .AddOptions<OtlpExporterOptions>()
                .Configure<IConfiguration>((options, conifg) =>
                {
                    var nodeIp = conifg.GetValue<string>("NODE_IP");

                    if (!string.IsNullOrWhiteSpace(nodeIp))
                        options.Endpoint = new Uri($"http://{nodeIp}:4317");

                    conifg.GetSection("Opentelemetry:Tracing").Bind(options);
                });

            services
                .AddOpenTelemetryTracing(
                    builder => builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddEnvironmentVariableDetector())
                        .AddSource("Tgs.*")
                        .AddSource("TGS.*")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                        })
                        .AddProfileViewExporter(_ => { })
                        .AddOtlpExporter())
                .Configure<AspNetCoreInstrumentationOptions>(options =>
                {
                    options.Filter = httpContext =>
                    {
                        return !httpContext.Request.Path.HasValue
                            || !httpContext.Request.Path.StartsWithSegments("/swagger")
                            && !httpContext.Request.Path.StartsWithSegments("/health")
                            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                            && !new[]
                            {
                                "html",
                                "ico",
                                "png",
                                "jpg"
                            }.Any(ext => httpContext.Request.Path.Value?.EndsWith($".{ext}") == true);
                    };
                });
            services.AddProfileViewer();
        }
    }
}
