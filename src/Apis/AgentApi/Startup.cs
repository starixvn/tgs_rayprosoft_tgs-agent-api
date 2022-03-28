using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Filters;
using TGS.Apis.AgentApi.Filters.Swaggers;
using TGS.Apis.AgentApi.Models;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;
using TGS.Services.PluggableServices.Extensions;
using GlobalConsts = TGS.Core.Models.Consts;

namespace TGS.Apis.AgentApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTgsErrorHandler();

            app.UseRequestLocalization(options =>
            {
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                options.SupportedCultures.Clear();
                options.SupportedUICultures.Clear();

                config.GetSection("RequestLocalization").Bind(options);

                options.SetDefaultCulture(config.GetValue<string>("RequestLocalization:DefaultRequestCulture"));
            });

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
                endpoints.MapSwagger();
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Normal/swagger.json", "TGS Agent API");
                c.SwaggerEndpoint("/swagger/HK28/swagger.json", "TGS Agent API - HK28");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs");

            services
                .AddWebApiServices()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddOperations()
                .AddErrorMappers()
                .AddApiClient()
                .AddPluggableServices()
                .AddReportClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Report:Url");
                })
                .AddTransactions();

            services.AddErrorMapper()
                .AddErrorMapper<ApiErrorMapper>();

            services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            services
                .AddAgentApiCore()
                .ConfigureAgentOptions<IConfiguration>((options, config) =>
                {
                    config.GetSection("Agent").Bind(options);
                })
                .ConfigureEmailConfiguration<IConfiguration>((options, config) =>
                {
                    config.GetSection("EmailSetting").Bind(options);
                })
                .AddPaymentServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.PaymentServiceBaseUrl = config.GetValue<Uri>("Agent:PaymentServiceBaseUrl");

                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter() }
                    };
                })
                .ConfigureWallet(config =>
                {
                    config.AddTgsAgentWalletRepositories();
                })
                .AddTgsAgentRepository();

            services.AddHttpClient<IEmailClient, EmailClient>((sp, c) =>
            {
                var url = _configuration.GetSection("Email:Url");
                c.BaseAddress = new Uri(url.Value);
            });
            services.AddMemoryCache();

            services.AddRouting(options => options.LowercaseUrls = true);

            services
                .AddControllers(options =>
                {
                    options.Filters.Add<CheckAgentEnabledFilter>();
                    options.Filters.Add<ValidateModelFilter>();
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

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(Startup).Assembly);

            services
                .AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddRedis(_configuration)
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
                });

            services.AddSwaggerGen(c =>
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

                c.DocInclusionPredicate((groupName, desc) =>
                {
                    if (groupName == "Normal")
                        return desc.GroupName is null;
                    return desc.GroupName == groupName;
                });

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
                            && !new[]
                            {
                                "html",
                                "ico",
                                "png",
                                "jpg"
                            }.Any(
                                ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
                    };
                });
            services.AddProfileViewer();
        }
    }
}