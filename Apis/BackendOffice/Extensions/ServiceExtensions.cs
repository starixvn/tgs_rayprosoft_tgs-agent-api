using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TGS.Apis.BackendOffice.DataService.DataInitializers;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Filters;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackendOfficeServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddRepository<IBackendOfficeRepository, BackendOfficeRepository>(configuration, "BackendOffice");
            services.AddRepository<ITgsRepository, TgsRepository>(configuration, "Tgs");
            services.AddScoped<ITgsHashConfig, TgsHashConfig>();
            services.AddScoped<IHashProvider, HashProvider>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();
            services.AddTransient(typeof(IBackendOfficeCommands<>), typeof(BackendOfficeCommands<>));
            services.AddTransient(typeof(IBackendOfficeQueries<>), typeof(BackendOfficeQueries<>));
            services.AddTransient(typeof(ITgsCommands<>), typeof(TgsCommands<>));
            services.AddTransient(typeof(ITgsQueries<>), typeof(TgsQueries<>));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddNats(configuration);
            services.AddWebApiServices();
            services.AddTgsLog();
            services.AddScoped<ExceptionMiddleware>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddHttpContextAccessor();
            services.AddSingleton<IFilterFactory, AutoValidateAntiforgeryTokenAttribute>();
            services.AddControllers()
                .AddFluentValidation(opt =>
                {
                    opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });

            services.AddHealthChecks()
                .AddCheck<HealthCheck>("example_health_check");

            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.HeaderName = Consts.CsrfTokenName;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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


            var enUSCulture = "en-US";
            var zhCNCulture = "zh-CN";

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo(enUSCulture),
                    new CultureInfo(zhCNCulture)
                };

                options.DefaultRequestCulture = new RequestCulture(culture: zhCNCulture, uiCulture: zhCNCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context => await Task.FromResult(new ProviderCultureResult(zhCNCulture))));
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new ValidationFilter());

                //// TODO: It seems AutoValidateAntiforgeryTokenAttribute not work,
                //// we need to implement it on our self if not done before deployment
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddDataAnnotationsLocalization();

            if (env.IsDevelopmentOrDocker())
            {
                services.AddScoped<IDataInitializer, DataInitializer>();
            }

            return services;
        }
    }
}
