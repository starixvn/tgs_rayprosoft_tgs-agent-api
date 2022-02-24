using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.Payment.ApplePay;
using Tgs.Payment.Help2Pay;
using Tgs.Payment.LeePay;
using Tgs.Payment.MaticPay;
using Tgs.Payment.PrimePay;
using Tgs.Payment.SkyPay;
using Tgs.Payment.UmPay;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Filters;
using TGS.Apis.OperatorApi.Filters.Swaggers;
using TGS.Apis.OperatorApi.Models;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operations.Extensions;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;
using TGS.Services.Operator.ValidateCode;
using GlobalConsts = TGS.Core.Models.Consts;

namespace TGS.Apis.OperatorApi
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.UseForwardedHeaders();
            app.Use((httpContext, next) =>
            {
                var forwardedIps = string.Join(
                    ", ",
                    httpContext.Request.Headers["X-Forwarded-For"].Select(ip => ip));
                logger.LogInformation(
                    $"Request Ip: {httpContext.Connection.RemoteIpAddress}, " +
                    $"Forward-For: {forwardedIps}");

                return next();
            });

            app.UseRequestLocalization(options =>
            {
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                options.SupportedCultures.Clear();
                options.SupportedUICultures.Clear();

                config.GetSection("RequestLocalization").Bind(options);

                options.SetDefaultCulture(config.GetValue<string>("RequestLocalization:DefaultRequestCulture"));
            });

            app.UseTgsErrorHandler();
            app.UseBrandCertificate();
            app.UseApiLogger(bypassRouting: Consts.DefaultBypassRouting);

            app.UseRouting();

            app.UseHttpMetrics();
            app.UseCors();

            app.UseVerifyJwtTokenMiddlewares(Consts.DefaultBypassRouting);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health",
                    new HealthCheckOptions
                    {
                        ResultStatusCodes = GlobalConsts.HealthCheckResultStatusCodes,
                        ResponseWriter = (httpContext, result) =>
                            HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                    });
                endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Operator API"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions<ForwardedHeadersOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                    options.ForwardLimit = config.GetValue<int>("ForwardedHeaders:ForwardLimit");

                    var forwardedForHeaderName = config.GetValue<string>("ForwardedHeaders:ForwardedForHeaderName");
                    if (!string.IsNullOrWhiteSpace(forwardedForHeaderName))
                        options.ForwardedForHeaderName = forwardedForHeaderName;

                    var knownProxies = config.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ??
                        Array.Empty<string>();

                    foreach (var knownProxie in knownProxies)
                        options.KnownProxies.Add(IPAddress.Parse(knownProxie));

                    var knownNetworks = config.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ??
                        Array.Empty<string>();

                    foreach (var cidr in knownNetworks)
                    {
                        (var ip, var range) = cidr.Split("/");

                        options.KnownNetworks.Add(
                            new IPNetwork(
                                IPAddress.Parse(ip),
                                string.IsNullOrEmpty(range)
                                    ? 32
                                    : int.Parse(range)));
                    }
                });

            services.AddTgsRepository(_configuration, "Tgs");

            services
                .AddWebApiServices()
                .AddTgsLog()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddOperations()
                .AddApiClient()
                .AddReportClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Report:Url");
                })
                .AddPromotionClientServices((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    config.GetSection("Promotion").Bind(options);
                })
                .AddTransactions()
                .AddBrandCertificateClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BrandCertificateBaseUrl = config.GetValue<Uri>("Operator:BrandCertificateBaseUrl");
                })
                .AddGameProvidersPluggableServerClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Operator:GameProvidersPluggableBaseUrl");
                });

            services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            services.AddErrorMapper()
                .AddErrorMapper<OperatorApiErrorMapper>();

            services
                .AddOperatorApiCore()
                .ConfigureOperatorOptions<IConfiguration>((options, config) =>
                {
                    config.GetSection("Operator").Bind(options);
                })
                .UseOperatorRepositories(sp
                    => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
                .ConfigureWallet((options, sp) =>
                {
                    var operatorOptions = sp.GetRequiredService<IOptions<OperatorOptions>>().Value;

                    options.BaseUrl = operatorOptions.CashBaseUrl;
                })
                .AddPaymentServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.PaymentServiceBaseUrl = config.GetValue<Uri>("Operator:PaymentServiceBaseUrl");

                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter() }
                    };
                })
                .Services
                .AddSingleton<ManualDepositSerializer>()
                .AddSingleton<PrimePaySerializer>()
                .AddSingleton<ApplePaySerializer>()
                .AddSingleton<LeePaySerializer>()
                .AddSingleton<UmPaySerializer>()
                .AddSingleton<MaticPaySerializer>()
                .AddSingleton<Help2PaySerializer>()
                .AddSingleton<SkyPaySerializer>();

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
                .AddOptions<ConstValidateCodeOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    config.GetSection("Operator").Bind(options);
                })
                .Services
                .Decorate<IValidateCodeGenerator, ConstValidateCodeGenerator>()
                .Decorate<IGetService<GetPlayerQuery, PlayerInfo>, MemoryGetPlayerQueryService>();

            services
                .AddIdentityVerifiers()
                .AddPlayerBucketRepositoriesServices();

            services
                .AddRouting(options => options.LowercaseUrls = true);

            services
                .AddControllers(options =>
                {
                    options.AddBrandCertificateFilters();
                    options.Filters.Add<CheckPlayerEnabledFilter>();
                    options.Filters.Add<ValidateModelFilter>();
                    options.AddWebApiToolkitFilters();
                    options.Filters.Add<TgsResponseWrapperAttribute>();
                })
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Startup>(lifetime: ServiceLifetime.Scoped);
                    c.DisableDataAnnotationsValidation = true;
                    c.LocalizationEnabled = false;
                    c.ImplicitlyValidateChildProperties = true;
                });

            services.AddHttpClient<IEmailClient, EmailClient>((sp, c) =>
            {
                var url = _configuration.GetSection("Email:Url");
                c.BaseAddress = new Uri(url.Value);
            });

            services.AddHttpContextAccessor();

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddRedis(_configuration)
                .ForwardToPrometheus();

            services
                .AddOptions<OtlpExporterOptions>()
                .Configure<IConfiguration>((options, conifg) =>
                {
                    var nodeIp = conifg.GetValue<string>("NODE_IP");

                    if (!string.IsNullOrWhiteSpace(nodeIp))
                        options.Endpoint = new Uri($"http://{nodeIp}:4317");

                    conifg.GetSection("Opentelemetry:Tracing").Bind(options);
                });
            services
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
                        .AddProfileViewExporter()
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
                });

            services.AddProfileViewer();

            services
                .AddInterceptionService()
                .AddServiceUtiltiyInterceptors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "TGS Operator API",
                        Version = "v1"
                    });

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
            services.AddCors(options =>
            {
                var corsDomain = _configuration.GetSection("CorsDomain").Get<CorsDomain>();

                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(corsDomain.Urls)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true);
                    });
            });
        }
    }
}
