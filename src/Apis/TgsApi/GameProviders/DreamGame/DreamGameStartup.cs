using System;
using System.Collections.Generic;
using System.Net.Mime;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.Interceptors;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Constraints;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Middlewares;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Services;
using TGS.Core.DataServices.Tgs.Extensions;
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

namespace TGS.Apis.TgsApi.GameProviders.DreamGame
{
    public class DreamGameStartup
    {
        private readonly IConfiguration _configuration;

        public DreamGameStartup(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGlobalExceptionHandler();

            app.UseGameApiLogger(
                "wmcasino",
                new[] { "/health", "/favicon.ico", "/metrics" });

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseWhen(
                context =>
                {
                    if (context.Request.ContentType != MediaTypeNames.Application.Json)
                    {
                        return false;
                    }

                    var route = "/api/dg";

                    return context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase);
                },
                builder =>
                {
                    builder.UseMiddleware<HandleDreamGameExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeDreamGameMiddleware>();
                });

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
            _ = services.AddTgsRepository(_configuration, "Tgs");

            _ = services
                .AddWebApiServices()
                .AddTgsLog()
                .AddSystemJsonSerializer()
                .AddTransactions()
                .AddOperations()
                .AddTimeZone()
                .AddGameOperations(sp
                    => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
                .AddGameProvidersoRepository();

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
            /*
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
            */
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
                        .AddProfileViewExporter(profileViewBuilder =>
                        {
                            profileViewBuilder
                                .AddFilter(new FileExtensionFilter("html", "ico", "png", "jpg", "js"))
                                .AddFilter(new UrlPathFilter("/health"));
                        }))
                .AddProfileViewer();

            _ = services
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

            _ = services.AddErrorMappers(new[]
                {
                    typeof(DreamGameErrorMapper)
                },
                (provider, key) => key switch
                {
                    Consts.GameProvider.WMCasino => provider.GetRequiredService<DreamGameErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            _ = services.AddScoped<AuthorizeDreamGameMiddleware>();
            _ = services.AddScoped<HandleDreamGameExceptionMiddleware>();

            _ = services.AddScoped<IDreamGameService, DreamGameService>();

            _ = services.AddScoped<IDreamGamegModelActionMapper, DreamGamegModelActionMapper>();

            _ = services.Configure<RouteOptions>(routeOptions =>
              {
                  routeOptions.ConstraintMap.Add("RegexAgentName", typeof(RegexAgentNameConstraint));
              });
        }
    }
}
