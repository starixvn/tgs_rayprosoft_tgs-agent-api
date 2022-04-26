using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using TGS.Apis.BackendOffice.Filters;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;

namespace TGS.Apis.BackendOffice
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
            => _configuration = configuration;

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization(options =>
            {
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                options.SupportedCultures.Clear();
                options.SupportedUICultures.Clear();

                config.GetSection("RequestLocalization").Bind(options);

                options.SetDefaultCulture(config.GetValue<string>("RequestLocalization:DefaultRequestCulture"));
            });

            if (env.IsDevelopmentOrDocker())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapProfileViewer();
                endpoints.MapHealthChecks("/health");

                endpoints.MapMetrics();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS BackOffice API"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddDataProtection();
            _ = services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(sp
                => new ConfigureNamedOptions<KeyManagementOptions>(
                    string.Empty,
                    options =>
                    {
                        options.XmlRepository = new RedisXmlRepository(
                            () => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase(),
                            "Tgs:Bo-Api:DataProtection");
                    }));

            _ = services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            _ = services
                .AddBackendOfficeServices(_configuration)
                .AddBackendOfficeRepositories(
                    sp => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase())
                .AddMessageQueue()
                .AddNatsMessageQueue(config =>
                {
                    _ = config
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var configuration = sp.GetRequiredService<IConfiguration>();

                            options.Url = configuration.GetValue<string>("NatsSetting:Urls");
                            options.MaxReconnect = -1;
                        })
                        .AddReplyHandler<AutoApproveDepositProcessor>(
                            "tgs.payment.approve",
                            "tgs.payment.approve")
                        .AddReplyHandler<AutoApproveDepositProcessor>(
                            "tgs.payment.deposit.approve",
                            "tgs.payment.deposit.approve")
                        .AddReplyHandler<AutoRejectDepositProcessor>(
                            "tgs.payment.reject",
                            "tgs.payment.reject")
                        .AddReplyHandler<AutoRejectDepositProcessor>(
                            "tgs.payment.deposit.reject",
                            "tgs.payment.deposit.reject")
                        .AddReplyHandler<AutoRejectWithdrawProcessor>(
                            "tgs.payment.withdraw.reject",
                            "tgs.payment.withdraw.reject");
                })
                .AddNatsGlobPatternExchange("*");

            _ = services.AddPaymentServiceClient((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.PaymentServiceBaseUrl = config.GetValue<Uri>("BackOffice:PaymentServiceBaseUrl");

            });

            _ = services.AddLocalization(options => options.ResourcesPath = "Resources");

            _ = services
                .AddHttpContextAccessor()
                .AddRouting(options => options.LowercaseUrls = true);

            _ = services
                .AddControllers(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add(new ValidationFilter());
                })
                .AddDataAnnotationsLocalization()
                .AddFluentValidation(c =>
                {
                    _ = c.RegisterValidatorsFromAssembly(
                        Assembly.GetExecutingAssembly(),
                        lifetime: ServiceLifetime.Scoped);
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            _ = services
                .AddHealthChecks()
                .ForwardToPrometheus();

            _ = services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.HeaderName = Consts.BackendOffice.CsrfTokenName;
            });

            _ = services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/api/auth");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = 401;

                        return Task.CompletedTask;
                    };
                });

            _ = services.AddFileServiceClient((sp, httpClient) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                httpClient.BaseAddress = new Uri(config.GetValue<string>("File:FileServiceBaseUrl"));
            });

            _ = services.AddGameProvidersPluggableServerClient((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.BaseUrl = config.GetValue<Uri>("BackOffice:GameProvidersPluggableBaseUrl");
            });

            _ = services.AddDomainSettingClient((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.DomainSettingBaseAddress = config.GetValue<Uri>("BackOffice:DomainSettingBaseAddress");
            });

            _ = services.AddPlayerServiceClient((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                options.BaseUrl = config.GetValue<Uri>("BackOffice:PlayerServiceBaseUrl");
            });

            _ = services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "TGS BackOffice API",
                        Version = "v1"
                    });

                c.CustomSchemaIds(type => type.FullName.Replace('.', '_'));

                var xml = "TGS.Apis.BackendOffice.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);

                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });

            _ = services
                .AddOptions<OtlpExporterOptions>()
                .Configure<IConfiguration>((options, conifg) =>
                {
                    var nodeIp = conifg.GetValue<string>("NODE_IP");

                    if (!string.IsNullOrWhiteSpace(nodeIp))
                        options.Endpoint = new Uri($"http://{nodeIp}:4317");

                    conifg.GetSection("Opentelemetry:Tracing").Bind(options);
                });

            _ = services
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

            _ = services.AddProfileViewer();
        }
    }
}
