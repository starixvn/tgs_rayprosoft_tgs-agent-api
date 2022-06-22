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
using TGS.Apis.TgsApi.GameProviders.Saba.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
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

namespace TGS.Apis.TgsApi.GameProviders.Saba;

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
            .AddGameOperations(sp
                => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
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

        // custom registration of Saba

        _ = services.AddErrorMappers(
            new[]
            {
                typeof(SabaErrorMapper)
            },
            (provider, key) => key switch
            {
                GameProvider.Saba => provider.GetRequiredService<SabaErrorMapper>(),
                _ => provider.GetRequiredService<TgsErrorMapper>()
            });

        _ = services.AddScoped<AuthorizeSabaMiddleware>();
        _ = services.AddScoped<HandleSabaExceptionMiddleware>();

        _ = services.AddSingleton<GzipRequestDecompressionMiddleware>();

        _ = services.AddScoped<ISabaServices, SabaServices>();
        _ = services.AddScoped<ISabaActionModelMapper, SabaActionModelMapper>();

        _ = services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(SabaGameInitializer)
            },
            (provider, _) => provider.GetRequiredService<SabaGameInitializer>());

        _ = services.AddScopedWithResolver<IGameHistory>(new[]
            {
                typeof(SabaGameHistory)
            },
            (provider, _) => provider.GetRequiredService<SabaGameHistory>());
    }
}
