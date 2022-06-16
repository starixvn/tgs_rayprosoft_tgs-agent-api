using System;
using System.Collections.Generic;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using TGS.Apis.TgsApi.GameProviders.Skywind.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.Skywind;

public sealed class SkywindStartup
{
    private readonly IConfiguration _configuration;

    public SkywindStartup(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        _ = app.UseGlobalExceptionHandler();

        _ = app.UseGameApiLogger(
            "skywind",
            new[] { "/health", "/favicon.ico", "/metrics" });

        _ = app.UseWhen(
            context =>
                context.Request.Path.StartsWithSegments("/sw/api", StringComparison.OrdinalIgnoreCase),
            builder =>
                {
                    builder.UseMiddleware<HandleSkywindExceptionMiddleware>();
                });

        _ = app.UseRouting();

        _ = app.UseHttpMetrics();

        _ = app.UseWhen(context =>
                context.Request.Path.StartsWithSegments("/sw/api", StringComparison.OrdinalIgnoreCase),
            builder =>
                {
                    _ = builder.UseMiddleware<AuthorizeSkywindMiddleware>();
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
        }).AddXmlSerializerFormatters().ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = false;
        });

        _ = services.AddHealthChecks()
            .ForwardToPrometheus();

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
            .AddMessageQueue()
            .AddNatsMessageQueue(natsConfig =>
            {
                natsConfig
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var logger = sp.GetRequiredService<ILogger<SkywindStartup>>();
                        var config = sp.GetRequiredService<IConfiguration>();
                        var natsUrl = config.GetValue<string>("NatsSetting:Urls");

                        logger.LogInformation($"nats url: {natsUrl}");

                        options.Url = natsUrl;
                    });
            })
            .AddNatsGlobPatternExchange("*");

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

        _ = services.AddErrorMappers(new[]
            {
                typeof(SkywindErrorMapper)
            },
            (provider, key) => key switch
            {
                Consts.GameProvider.Skywind => provider.GetRequiredService<SkywindErrorMapper>(),
                _ => provider.GetRequiredService<TgsErrorMapper>()
            });

        _ = services.AddScoped<AuthorizeSkywindMiddleware>();
        _ = services.AddScoped<HandleSkywindExceptionMiddleware>();
        _ = services.AddScoped<ISkywindService, SkywindService>();
        _ = services.AddScoped<ISkywindActionModelMapper, SkywindActionModelMapper>();
    }
}
