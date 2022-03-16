using System;
using System.Collections.Generic;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter;
using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.Interceptors;
using Tgs.MessageQueue.MongoDB.Configuration;
using Tgs.MessageQueue.Nats.Configuration;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Operations.Extensions;
using MongoMessageQueue = MongoDB.Messaging.MessageQueue;


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

                endpoints.MapHealthChecks("/health",
                    new HealthCheckOptions
                    {
                        ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                        ResponseWriter = (httpContext, result) =>
                            HealthCheckResponseWriter.CreateResponseAsync(httpContext,
                                result,
                                Environment,
                                async sp =>
                                {
                                    var gpCode =
                                        Configuration.GetValue<string>(Consts.EnvironmentVariable.GameProviderCode);

                                    var feature = sp.GetRequiredService<IFeatureManager>();

                                    var isFeatureEnabled = gpCode.IsNotNullOrEmpty() &&
                                                           await feature.IsEnabledAsync(gpCode);

                                    return new Dictionary<string, object>
                                    {
                                        { "GameProviderCode", gpCode }, { "FeatureEnabled", isFeatureEnabled }
                                    };
                                })
                    });

                endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(Configuration, "Tgs");

            services
                .AddWebApiServices()
                .AddTgsLog()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddTransactions()
                .AddOperations()
                .AddTimeZone()
                .AddApiClient()
                .AddGameApiClients()
                .AddGameOperations((sp) => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
                .AddGameProvidersoRepository()
                .AddCashServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Game:CashBaseUrl");
                });

            services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            services.AddHttpContextAccessor();

            AddCustomDependencyServices(services);

            services.AddFeatureManagement();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
                AddCustomFilters(options);
            }).AddXmlSerializerFormatters().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });

            services.AddHealthChecks()
                .AddPostgres(Configuration, "Tgs", "tgs-db")
                .AddRedis(Configuration)
                .ForwardToPrometheus();

            services
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

            //services
            //    .AddOptions<OtlpExporterOptions>()
            //    .Configure<IConfiguration>((options, config) =>
            //    {
            //        var nodeIp = config.GetValue<string>("NODE_IP");

            //        if (!string.IsNullOrWhiteSpace(nodeIp))
            //        {
            //            options.Endpoint = new Uri($"http://{nodeIp}:4317");
            //        }

            //        config.GetSection("OpenTelemetry:Tracing").Bind(options);
            //    });

            services
                .AddOpenTelemetryTracing(builder
                    => builder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
                        .AddSource("TGS.*")
                        .AddSource("Tgs.*")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                        })
                        .AddOtlpExporter()
                        .AddProfileViewExporter(profileViewBuilder =>
                        {
                            profileViewBuilder
                                .AddFilter(new FileExtensionFilter("html", "ico", "png", "jpg", "js"))
                                .AddFilter(new UrlPathFilter("/health"));
                        }))
                .AddProfileViewer();

            services
                .AddInterceptionService()
                .ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<DiagnosticsInterceptor>(
                            Predicates.ForNameSpace("Tgs.*"),
                            Predicates.ForNameSpace("TGS.*"));

                    config.NonAspectPredicates
                        .AddNamespace("Tgs.*Infrastructures.*Json*")
                        .AddNamespace("TGS.*Infrastructures.*Json*")
                        .AddNamespace("Tgs.Logging*")
                        .AddNamespace("TGS.Logging*");
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
