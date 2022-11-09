using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Filters.Swaggers;
using TGS.Apis.AgentApi.Filters;
using TGS.Apis.AgentApi.Models;
using TGS.Apis.AgentApi;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;
using Prometheus;
using TGS.Core.Infrastructures.Log;
using TGS.Apis.AgentApi.Extensions;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

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
    .ConfigureEmailConfiguration<IConfiguration>((options, config) =>
    {
        config.GetSection("EmailSetting").Bind(options);
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

builder.Services.AddHttpClient<IEmailClient, EmailClient>((sp, c) =>
{
    var url = builder.Configuration.GetSection("Email:Url");
    c.BaseAddress = new Uri(url.Value);
});

builder.Services.AddMemoryCache();

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        _ = options.Filters.Add<CheckAgentEnabledFilter>();
        _ = options.Filters.Add<ValidateModelFilter>();
        _ = options.Filters.Add<TgsResponseWrapperAttribute>();
    })
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
    .AddFluentValidation(c =>
    {
        _ = c.RegisterValidatorsFromAssemblyContaining<Program>(lifetime: ServiceLifetime.Scoped);
        c.DisableDataAnnotationsValidation = true;
    });

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
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
    var corsDomain = builder.Configuration.GetSection("CorsDomain").Get<CorsDomain>();

    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(corsDomain.Urls)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true);
        });
});

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, conifg) =>
    {
        var nodeIp = conifg.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        conifg.GetSection("Opentelemetry:Tracing").Bind(options);
    })
    .Services
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
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    })
    .AddProfileViewer()
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services.AddSwaggerGen(c =>
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

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Api Authorization",
        Description = "authorized jwt token, get from api/auth/jwt",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.DocInclusionPredicate((groupName, desc)
        => groupName == "Normal" ? desc.GroupName is null : desc.GroupName == groupName);

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

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
app.UseVerifyJwtTokenMiddlewares(Consts.DefaultBypassRouting);

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
