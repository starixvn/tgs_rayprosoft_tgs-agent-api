using System;
using System.Collections.Generic;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.Interceptors;
using Tgs.MessageQueue.MongoDB.Configuration;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Middlewares;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;
using static TGS.Core.Models.Consts;
using MongoMessageQueue = MongoDB.Messaging.MessageQueue;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct
{
    public class BetConstructStartup
    {
        private readonly IConfiguration _configuration;

        public BetConstructStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseGlobalExceptionHandler();

            _ = app.UseGameApiLogger(
                "betconstruct",
                new[] { "/health", "/favicon.ico", "/metrics" });

            _ = app.UseRouting();

            _ = app.UseHttpMetrics();

            _ = app.UseWhen(
                context =>
                {
                    var route = "/bc/api";

                    return context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase);
                },
                builder =>
                {                    
                    builder.UseMiddleware<AuthorizeBetConstructMiddleware>();
                });

            _ = app.UseEndpoints(endpoints =>
              {
                  endpoints.MapControllers();

                  endpoints.MapHealthChecks("/health");

                  endpoints.MapMetrics();
                  endpoints.MapProfileViewer();
              });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddTgsRepository(_configuration, "Tgs");

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

            _ = services.AddFeatureManagement();

            _ = services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
            });

            _ = services.AddHealthChecks()
               .ForwardToPrometheus();

            _ = services
                .AddMessageQueue()
                .AddMongoMessageQueue(config =>
                {
                    _ = config
                    .DefineMessageQueue(
                        "gp.gamerecord.database",
                        sp => new MongoMessageQueueBuilderOptions
                        {
                            MessageQueue = MongoMessageQueue.Default,
                            ConnectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("MongoDBMessagingConnectionString")
                        });
                })
                .ClearAllExchanges()
                .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.detail.bc", "gp.gamerecord.database", "tgs.gp.gamerecord.database.detail.bc");

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
                        .AddSource("TGS.*")
                        .AddSource("Tgs.*")
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
                    config.Interceptors.AddTyped<DiagnosticsInterceptor>(
                            Predicates.ForNameSpace("Tgs.*"),
                            Predicates.ForNameSpace("TGS.*"));
                });

            _ = services.AddErrorMappers(
                new[]
                {
                    typeof(BetConstructErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.BetConstruct => provider.GetRequiredService<BetConstructErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            _ = services.AddScoped<AuthorizeBetConstructMiddleware>();
            _ = services.AddScoped<IBetConstructService, BetConstructService>();
            _ = services.AddScoped<IBetConstructActionModelMapper, BetConstructActionModelMapper>();
            _ = services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(BetConstructGameInitializer)
                },
                (provider, _) => provider.GetRequiredService<BetConstructGameInitializer>());

            _ = services.AddScopedWithResolver<IGameHistory>(new[]
                {
                    typeof(BCGameHistory)
                },
                (provider, _) => provider.GetRequiredService<BCGameHistory>());
        }
    }
}
