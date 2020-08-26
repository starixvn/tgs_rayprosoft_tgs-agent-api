using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.BackendOffice.DataService;
using TGS.Apis.BackendOffice.DataService.DataInitializers;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Core.Models;
using TGS.Infrastructures.Security;
using TGS.Services.Operations.Extensions;

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

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                //options.Filters.Add(new HttpResponseExceptionFilter());
            });
            return services;
        }
    }
}
