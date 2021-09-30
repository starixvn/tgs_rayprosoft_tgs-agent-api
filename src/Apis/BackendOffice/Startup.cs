using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Filters;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Models;

namespace TGS.Apis.BackendOffice
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

                endpoints.MapHealthChecks("/health",
                    new()
                    {
                        ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                        ResponseWriter = (httpContext, result) =>
                            HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, env)
                    });

                endpoints.MapMetrics();
            });

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS BackOffice API"));
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddBackendOfficeServices(_configuration)
                .AddBackendOfficeRepositories()
                .AddNatsMessageQueue(config =>
                {
                    config
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var configuration = sp.GetRequiredService<IConfiguration>();

                            options.Url = configuration.GetValue<string>("NatsSetting:Urls");
                        })
                        .AddReplyHandler<AutoApproveDepositProcessor>("tgs.payment.approve", "tgs.payment.approve")
                        .ProcessAllSubjects();
                }); ;

            services.AddPaymentServiceClient((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.PaymentServiceBaseUrl = config.GetValue<Uri>("BackOffice:PaymentServiceBaseUrl");

                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                };
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services
                .AddHttpContextAccessor()
                .AddRouting(options => options.LowercaseUrls = true);

            services
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
                    c.RegisterValidatorsFromAssembly(
                        Assembly.GetExecutingAssembly(),
                        lifetime: ServiceLifetime.Scoped);
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddNats()
                .ForwardToPrometheus();

            services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.HeaderName = Consts.BackendOffice.CsrfTokenName;
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new("/api/auth");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = 401;

                        return Task.CompletedTask;
                    };
                });

            services.AddFileServiceClient((sp, httpClient) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                httpClient.BaseAddress = new(config.GetValue<string>("File:FileServiceBaseUrl"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new()
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
        }
    }
}