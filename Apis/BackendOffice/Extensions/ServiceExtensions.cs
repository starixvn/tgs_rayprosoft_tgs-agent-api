using System;
using System.Globalization;
using System.Threading.Tasks;
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
using TGS.Apis.BackendOffice.DataService;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.DataInitializers;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackendOfficeServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepository<IBackendOfficeRepository, BackendOfficeRepository>(configuration, "BackendOffice");
            services.AddScoped<IDataInitializer, DataInitializer>();
            services.AddScoped<ITgsHashConfig, TgsHashConfig>();
            services.AddScoped<IHashProvider, HashProvider>();
            services.AddScoped<IUserQueries, UserQueries>();
            services.AddScoped<IUserCommands, UserCommands>();
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");


            services.AddSingleton<IFilterFactory, AutoValidateAntiforgeryTokenAttribute>();
            services.AddControllers();
            services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.HeaderName = Consts.CsrfTokenName;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/api/auth");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);//沒給預設14天
                });


            string enUSCulture = "en-US";
            string zhCNCulture = "zh-CN";

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
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                //options.Filters.Add(new HttpResponseExceptionFilter());
            }).AddDataAnnotationsLocalization(); ;
            return services;
        }
    }
}
