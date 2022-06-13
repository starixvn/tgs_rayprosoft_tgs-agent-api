using System;
using System.Collections.Generic;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.Interceptors;
using Tgs.Logging;
using Tgs.MessageQueue.Nats.Configuration;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi.Commons
{
    public abstract class BaseStartup
    {
        protected IConfiguration Configuration { get; }

        protected IWebHostEnvironment Environment { get; }

        protected abstract string LogPrefix { get; }

        protected BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseGameApiLogger(
                LogPrefix,
                new[] { "/health", "/favicon.ico", "/metrics" });

            UseCustomMiddlewaresBeforeRouting(app);

            app.UseRouting();

            app.UseHttpMetrics();

            UseCustomMiddlewaresAfterRouting(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health");

                endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddTgsRepository(Configuration, "Tgs");

            _ = services
                .AddWebApiServices()
                .AddTgsLog()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddTransactions()
                .AddOperations()
                .AddApiClient()
                .AddGameApiClients()
                .AddGameOperations((sp) => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
                .AddGameProvidersoRepository()
                .AddCashServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Game:CashBaseUrl");
                });

            _ = services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            _ = services.AddHttpContextAccessor();

            AddCustomDependencyServices(services);

            _ = services.AddFeatureManagement();

            _ = services
                .AddControllers(options =>
                {
                    options.Filters.Add<ValidateModelFilter>();
                    AddCustomFilters(options);
                })
                .AddXmlSerializerFormatters()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = false;
                });

            _ = services.AddHealthChecks();
            _ = services
                .AddMessageQueue()
                .AddNatsMessageQueue(natsConfig =>
                {
                    natsConfig
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var logger = sp.GetRequiredService<ILogger<BaseStartup>>();
                            var config = sp.GetRequiredService<IConfiguration>();
                            var natsUrl = config.GetValue<string>("NatsSetting:Urls");

                            logger.LogInformation($"nats url: {natsUrl}");

                            options.Url = natsUrl;
                        });

                    AddCustomNatsMessageQueueHandler(natsConfig);
                })
                .AddNatsGlobPatternExchange("*");

            _ = services
                .AddOptions<OtlpExporterOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    var nodeIp = config.GetValue<string>("NODE_IP");

                    if (!string.IsNullOrWhiteSpace(nodeIp))
                    {
                        options.Endpoint = new Uri($"http://{nodeIp}:4317");
                    }

                    config.GetSection("OpenTelemetry:Tracing").Bind(options);
                });

            _ = services
                .AddOpenTelemetryTracing(builder
                    => builder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
                        .AddSource(
                            "Tgs.Interceptors.DiagnosticsInterceptor",
                            "Tgs.MessageQueue.*")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                        })
                        .AddOtlpExporter()
                        .AddProfileViewExporter())
                .Configure<AspNetCoreInstrumentationOptions>(options =>
                {
                    options.Filter = httpContext
                        => !httpContext.Request.Path.HasValue
                        || !httpContext.Request.Path.StartsWithSegments("/swagger")
                        && !httpContext.Request.Path.StartsWithSegments("/health")
                        && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                        && !httpContext.Request.Path.StartsWithSegments("/metrics")
                        && !new[]
                        {
                            "html",
                            "ico",
                            "png",
                            "jpg"
                        }.Any(
                            ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
                })
                .AddProfileViewer();

            _ = services
                .AddInterceptionService()
                .ConfigureDynamicProxy(config =>
                {
                    var namespaceCheckTgs = Predicates.ForNameSpace("Tgs.*");
                    var namespaceCheckTGS = Predicates.ForNameSpace("TGS.*");
                    var tgsLogCheck = Predicates.Implement(typeof(ITgsLog));
                    var redisCheck = Predicates.Implement(typeof(IRedisConnectionFactory));
                    var jsonSerializerCheck = Predicates.Implement(typeof(IJsonSerializationProvider));

                    config.Interceptors.AddTyped<DiagnosticsInterceptor>(
                        m => namespaceCheckTGS(m)
                            && namespaceCheckTgs(m)
                            && !tgsLogCheck(m)
                            && !redisCheck(m)
                            && !jsonSerializerCheck(m));
                });
        }

        #region Abstract and Virtual Methods

        protected virtual void AddCustomDependencyServices(IServiceCollection services)
        {
        }

        protected virtual void AddCustomFilters(MvcOptions options)
        {
        }

        protected virtual void AddCustomNatsMessageQueueHandler(NatsMessageQueueConfiguration configuration)
        {
        }

        protected virtual void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
        }

        protected virtual void UseCustomMiddlewaresBeforeRouting(IApplicationBuilder app)
        {
        }

        #endregion Abstract and Virtual Methods
    }
}
