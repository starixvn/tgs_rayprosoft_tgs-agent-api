using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.PlayerBucket.Clients.DependencyInjection;
using TGS.Apis.AgentApi;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Filters;
using TGS.Apis.AgentApi.Filters.Swaggers;
using TGS.Apis.AgentApi.Models;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Services
    .AddOptions<ForwardedHeadersOptions>()
    .Configure<IConfiguration>((options, config) =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
        options.ForwardLimit = config.GetValue<int>("ForwardedHeaders:ForwardLimit");

        var forwardedForHeaderName = config.GetValue<string>("ForwardedHeaders:ForwardedForHeaderName");
        if (!string.IsNullOrWhiteSpace(forwardedForHeaderName))
            options.ForwardedForHeaderName = forwardedForHeaderName;

        var knownProxies = config.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>();

        foreach (var knownProxie in knownProxies)
            options.KnownProxies.Add(IPAddress.Parse(knownProxie));

        var knownNetworks = config.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ??
            Array.Empty<string>();

        foreach (var cidr in knownNetworks)
        {
            var (ip, range) = cidr.Split("/");

            options.KnownNetworks.Add(
                new IPNetwork(
                    IPAddress.Parse(ip),
                    string.IsNullOrEmpty(range)
                        ? 32
                        : int.Parse(range)));
        }
    });

builder.Logging
    .AddTgsLog()
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

builder.Services
    .AddTgsRepository(builder.Configuration, "Tgs");

builder.Services
    .AddWebApiServices()
    .AddSystemJsonSerializer()
    .AddSecurity()
    .AddOperations()
    .AddErrorMappers()
    .AddApiClient()
    .AddReportClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.BaseUrl = config.GetValue<Uri>("Report:Url");
    })
    .AddBrandCertificateClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        options.BrandCertificateBaseUrl = config.GetValue<Uri>("Agent:BrandCertificateBaseUrl");
    })
    .AddBrandServiceClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        options.BrandCertificateBaseUrl = config.GetValue<Uri>("Agent:BrandCertificateBaseUrl");
    })
    .AddVipLevelServiceClient(httpBuilder => httpBuilder
        .ConfigureHttpClient(httpClient => httpClient.BaseAddress =
        builder.Configuration.GetValue<Uri>("Agent:VipLevelServiceBaseUrl")!))
    .AddTransactions();

builder.Services
    .AddErrorMapper()
    .AddErrorMapper<ApiErrorMapper>();

builder.Services
    .AddRedis((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
    });

builder.Services
    .AddAgentApiCore()
    .ConfigureAgentOptions<IConfiguration>((options, config) =>
    {
        config.GetSection("Agent").Bind(options);
    })
    .AddPaymentServiceClient(httpClientBuilder => httpClientBuilder
        .ConfigureHttpClient((sp, http) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            http.BaseAddress = config.GetValue<Uri>("Agent:PaymentServiceBaseUrl");
        }))
    .ConfigureWallet(config =>
    {
        config.AddTgsAgentWalletRepositories();
    })
    .AddTgsAgentRepository();

builder.Services.AddPlayerServiceClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.BaseUrl = config.GetValue<Uri>("Agent:PlayerServiceBaseUrl");
    })
    .AddAgentClient()
    .ConfigureHttpClient(httpBuilder => httpBuilder.BaseAddress =
        builder.Configuration.GetValue<Uri>("Agent:AgentServiceBaseUrl"));

builder.Services.AddHttpClient<IEmailClient, EmailClient>((sp, c) =>
{
    var url = builder.Configuration.GetSection("Email:Url");
    c.BaseAddress = new Uri(url.Value);
});

builder.Services.AddCashServiceClient((options, sp) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    options.BaseUrl = config.GetValue<Uri>("Agent:CashBaseUrl");
});

builder.Services.AddPlayerBucketClient(httpBuilder => { });

builder.Services.AddMemoryCache();

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.AddBrandCertificateFilters();
        _ = options.Filters.Add<CheckAgentEnabledFilter>();
        _ = options.Filters.Add<ValidateModelFilter>();
        _ = options.Filters.Add<TgsResponseWrapperAttribute>();
    })
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services
    .AddFluentValidationAutoValidation(c =>
        c.DisableDataAnnotationsValidation = true)
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<Program>(lifetime: ServiceLifetime.Scoped);

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(natsConfig =>
    {
        _ = natsConfig
               .ConfigQueueOptions((options, sp) =>
               {
                   var config = sp.GetRequiredService<IConfiguration>();

                   options.Url = config.GetValue<string>("NatsSetting:Urls");
               });
    })
    .AddNatsGlobPatternExchange("*");

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(r => r.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddCors(options =>
{
    var corsDomain = builder.Configuration.GetSection("CorsDomain").Get<CorsDomain>();

    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            _ = corsPolicyBuilder.WithOrigins(corsDomain.Urls)
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .SetIsOriginAllowed(_ => true);
        });
});

builder.Services
    .AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
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
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .WithMetrics()
    .Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, config) =>
    {
        var nodeIp = config.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        config.GetSection("Opentelemetry:Tracing").Bind(options);
    })
    .Services
    .Configure<AspNetCoreInstrumentationOptions>(options =>
    {
        options.Filter = httpContext =>
        {
            return !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/health")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    })
    .AddProfileViewer()
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc(
            "Normal",
            new OpenApiInfo
            {
                Title = "TGS Agent API",
                Version = "Normal"
            });
        c.SwaggerDoc(
            "HK28",
            new OpenApiInfo
            {
                Title = "Tgs Agent API - HK28",
                Version = "HK28"
            });

        c.DocInclusionPredicate((groupName, desc)
            => groupName == "Normal" ? desc.GroupName is null : desc.GroupName == groupName);

        c.AddBrandCertificateSecurityDefinition();
        c.OperationFilter<SwaggerHeaderFilter>();
        c.SchemaFilter<SwaggerEnumDescriptionFilter>();
        c.OperationFilter<SwaggerTgsResponseWrapperFilter>();

        var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var path = Path.Combine(AppContext.BaseDirectory, xml);

        if (File.Exists(path))
        {
            c.IncludeXmlComments(path);
        }
    });

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseForwardedHeaders();

app.UseTgsErrorHandler();

app.UseRequestLocalization(options =>
{
    var config = app.Services.GetRequiredService<IConfiguration>();

    options.SupportedCultures.Clear();
    options.SupportedUICultures.Clear();

    config.GetSection("RequestLocalization").Bind(options);

    _ = options.SetDefaultCulture(config.GetValue<string>("RequestLocalization:DefaultRequestCulture"));
});

app.UseApiLogger(bypassRouting: Consts.DefaultBypassRouting);

app.UseRouting();
app.UseCors();

app.UseVerifyBrandTokenMiddlewares(Consts.DefaultBypassRouting);

app.MapControllers();

app.MapHealthChecks("/health");
app.MapProfileViewer();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/Normal/swagger.json", "TGS Agent API");
    c.SwaggerEndpoint("/swagger/HK28/swagger.json", "TGS Agent API - HK28");
});

app.Run();
