using System;
using System.Collections.Generic;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter;
using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.Interceptors;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Constraints;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Services;
using TGS.Apis.TgsApi.GameProviders.Saba.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Operations.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Saba
{
    public sealed class SabaStartup
    {
        private readonly IConfiguration _configuration;

        public SabaStartup(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGlobalExceptionHandler();

            app.UseMiddleware<GzipRequestDecompressionMiddleware>();

            app.UseGameApiLogger(
                "saba",
                new[] { "/health", "/favicon.ico", "/metrics" });

            app.UseRouting();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/saba", StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleSabaExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeSabaMiddleware>();
                });

            app.UseHttpMetrics();

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
                                env,
                                async sp =>
                                {
                                    var configuration = sp.GetRequiredService<IConfiguration>();

                                    var gpCode =
                                        configuration.GetValue<string>(Consts.EnvironmentVariable.GameProviderCode);

                                    var feature = sp.GetRequiredService<IFeatureManager>();

                                    var isFeatureEnabled = gpCode.IsNotNullOrEmpty() &&
                                                           await feature.IsEnabledAsync(gpCode);

                                    return new Dictionary<string, object>
                                    {
                                        ["GameProviderCode"] = gpCode,
                                        ["FeatureEnabled"] = isFeatureEnabled
                                    };
                                })
                    });

                endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs");

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
                .AddGameOperations(sp
                    => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
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

            services.AddFeatureManagement();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
            });
            _ = services.Configure<RouteOptions>(routeOptions =>
            {
                routeOptions.ConstraintMap.Add("RegexAgentName", typeof(RegexAgentNameConstraint));
            });
            _ = services.AddScoped<IDreamGameService, NoopDreamGameService>();

            services.AddHealthChecks()
                //.AddPostgres(_configuration, "Tgs", "tgs-db")
                //.AddRedis(_configuration)
                .ForwardToPrometheus();

            services
                .AddMessageQueue()
                .AddNatsMessageQueue(natsConfig =>
                {
                    natsConfig
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var config = sp.GetRequiredService<IConfiguration>();

                            options.Url = config.GetValue<string>("NatsSetting:Urls");
                        });
                })
                .AddNatsGlobPatternExchange("*");

            services
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

            // custom registration of Saba

            services.AddErrorMappers(
                new[]
                {
                    typeof(SabaErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.Saba => provider.GetRequiredService<SabaErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeSabaMiddleware>();
            services.AddScoped<HandleSabaExceptionMiddleware>();

            services.AddSingleton<GzipRequestDecompressionMiddleware>();

            services.AddScoped<ISabaServices, SabaServices>();
            services.AddScoped<ISabaActionModelMapper, SabaActionModelMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(SabaGameInitializer)
                },
                (provider, _) => provider.GetRequiredService<SabaGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[]
                {
                    typeof(SabaGameHistory)
                },
                (provider, _) => provider.GetRequiredService<SabaGameHistory>());
        }
    }
}
