using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.OperatorApi.DataServices;
using TGS.Apis.OperatorApi.Middlewares;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.OperatorApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRepository<ITgsRepository, TgsRepository>(_configuration, "Tgs");
            
            services.AddTransient(typeof(ITgsCommands<>), typeof(TgsCommands<>));
            services.AddTransient(typeof(ITgsQueries<>), typeof(TgsQueries<>));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ICaptchaFactory, CaptchaFactory>();

            services.AddNats(_configuration)
                    .AddTgsLog()
                    .AddRedis()
                    .AddJson()
                    .AddSecurity()
                    .AddOperations()
                    .AddErrorMappers();

            services.AddHttpContextAccessor();

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

            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
