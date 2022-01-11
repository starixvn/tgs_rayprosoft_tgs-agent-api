using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Trace;
using Prometheus;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
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
using TGS.Services.GameOperations.Calculators;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino
{
    public class WMCasinoStartup
    {
        private readonly IConfiguration _configuration;

        public WMCasinoStartup(
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

            app.UseWhen(
                context =>
                    context.Request.Path.StartsWithSegments("/api/wm/entrance", StringComparison.OrdinalIgnoreCase) &&
                    HttpMethods.IsPost(context.Request.Method),
                builder =>
                {
                    builder.UseMiddleware<HandleWMCasinoExceptionMiddleware>();
                    builder.UseMiddleware<WMCasinoDispatcherMiddleware>();
                });

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseWhen(
                context =>
                {
                    if (context.Request.ContentType != MediaTypeNames.Application.Json)
                    {
                        return false;
                    }

                    var route = "/api/wm";

                    return context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase) &&
                           !context.Request.Path.StartsWithSegments($"{route}/gamerecord",
                               StringComparison.OrdinalIgnoreCase);
                },
                builder =>
                {
                    builder.UseMiddleware<AuthorizeWMCasinoMiddleware>();
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

            // custom registration of WMCasino

            services.AddScoped<WMCasinoDispatcherMiddleware>();
            services.AddScoped<AuthorizeWMCasinoMiddleware>();
            services.AddScoped<HandleWMCasinoExceptionMiddleware>();

            services.AddScoped<IWMCasinoServices, WMCasinoServices>();
            services.AddScoped<IWMCasinoBetActionServices, WMCasinoBetActionServices>();
            services.AddScoped<IWMCasinoGameProviderHistoryServices, WMCasinoGameProviderHistoryServices>();

            services.AddTransient<IWMCasinoAgentMapper, WMCasinoAgentMapper>();
            services.AddTransient<IWMCasinoUtility, WMCasinoUtility>();
            services.AddTransient<IWMCasinoPlayerServices, WMCasinoPlayerServices>();
            services.AddTransient<IWMCasinoActionModelMapper, WMCasinoActionModelMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(WMCasinoGameInitializer)
                },
                (provider, _) => provider.GetRequiredService<WMCasinoGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[]
                {
                    typeof(WMCasinoGameHistory)
                },
                (provider, _) => provider.GetRequiredService<WMCasinoGameHistory>());

            services.AddSingleton<IBetValuesCalculator, WMCasinoBetValuesCalculator>();

            services.AddTransient<IWMCasinoDataStore>(sp =>
                ActivatorUtilities.CreateInstance<WMCasinoRedisDataStore>(sp, sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase()));

            //services.AddHostedService<WMCasinoBetResultHandlerService>();

            services.AddFeatureManagement();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
            });

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
                        })
                        .AddReplyHandler<WMCasinoBetRequestInfoHandler>(
                            "tgs.gameprovider.wm.betdetailrequest.process",
                            "tgs.gameprovider.wm.betdetailrequest.process");
                });

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

            //services
            //    .AddOpenTelemetryTracing(builder
            //        => builder
            //            .SetResourceBuilder(
            //                ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
            //            .AddSource("TGS.*")
            //            .AddSource("Tgs.*")
            //            .AddAspNetCoreInstrumentation()
            //            .AddHttpClientInstrumentation()
            //            .AddEntityFrameworkCoreInstrumentation(options =>
            //            {
            //                options.SetDbStatementForText = true;
            //            })
            //            .AddOtlpExporter()
            //            .AddProfileViewExporter(profileViewBuilder =>
            //            {
            //                profileViewBuilder
            //                    .AddFilter(new FileExtensionFilter("html", "ico", "png", "jpg", "js"))
            //                    .AddFilter(new UrlPathFilter("/health"));
            //            }))
            //    .AddProfileViewer();

            services.AddErrorMappers(new[]
                {
                    typeof(WMCasinoErrorMapper)
                },
                (provider, key) => key switch
                {
                    Consts.GameProvider.WMCasino => provider.GetRequiredService<WMCasinoErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });
        }
    }
}
