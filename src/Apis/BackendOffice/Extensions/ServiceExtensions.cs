using System;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Filters;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Log;
using TGS.Core.DataServices.Report.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.BackOffice.Application.Extensions;
using TGS.Services.Operations.Extensions;
using TGS.Services.PluggableServices.Extensions;
using TGS.Services.Report.Application.Extensions;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackendOfficeServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRepository<ILogRepository, LogRepository>(configuration, "Log");
            services.AddTgsRepository(configuration, "Tgs");
            services.AddReportRepository(configuration, "Report");

            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<ExceptionMiddleware>();

            services.AddTransient<IStreamToString, StreamToString>();
            services.AddTransient(typeof(ITgsCommands<>), typeof(TgsCommands<>));
            services.AddTransient(typeof(ITgsQueries<>), typeof(TgsQueries<>));
            services.AddTransient(typeof(ILogQueries), typeof(LogQueries));
            services.AddTransient<IResellerService, ResellerService>();
            services.AddTransient<ILicenseeService, LicenseeService>();
            services.AddTransient<IBrandPaymentService, BrandPaymentService>();
            services.AddTransient<IStationLetterService, StationLetterService>();
            services.AddBackOfficeApplicationServices();
            services.AddReportServices();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IFilterFactory, AutoValidateAntiforgeryTokenAttribute>();

            services.AddNats(configuration)
                .AddJson()
                .AddSecurity()
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddOperations()
                .AddApiClient()
                .AddPluggableServices()
                .AddTransactions();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddHttpContextAccessor();
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
                    c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly(),
                        lifetime: ServiceLifetime.Singleton);
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddHealthChecks()
                .AddPostgres(configuration, "Tgs", "tgs-db")
                .AddPostgres(configuration, "Log", "log-db")
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

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var enUSCulture = "en-US";
                var zhCNCulture = "zh-CN";

                var supportedCultures = new[] {new CultureInfo(enUSCulture), new CultureInfo(zhCNCulture)};

                options.DefaultRequestCulture = new(zhCNCulture, zhCNCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(_ =>
                    Task.FromResult(new ProviderCultureResult(zhCNCulture))));
            });

            return services;
        }
    }
}