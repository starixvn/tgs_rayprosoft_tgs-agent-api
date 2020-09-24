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
using Microsoft.Extensions.Hosting;
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
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRepository<ITgsRepository, TgsRepository>(Configuration, "Tgs");
            services.AddScoped<ITgsHashConfig, TgsHashConfig>();
            services.AddScoped<IHashProvider, HashProvider>();
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();
            services.AddTransient(typeof(ITgsCommands<>), typeof(TgsCommands<>));
            services.AddTransient(typeof(ITgsQueries<>), typeof(TgsQueries<>));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ICaptchaFactory, CaptchaFactory>();

            services.AddNats(Configuration);
            services.AddTgsLog();
            services.AddScoped<AuthenticateMiddleware>();

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

            services.AddControllers();

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                //options.Filters.Add(new ValidationFilter());

                //// TODO: It seems AutoValidateAntiforgeryTokenAttribute not work,
                //// we need to implement it on our self if not done before deployment
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddErrorMappers(
                add: () =>
                {
                    services.AddSingleton<TgsErrorMapper>();
                },
                get: provider => key =>
                {
                    return key switch
                    {
                        _ => provider.GetRequiredService<TgsErrorMapper>()
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseMiddleware<AuthenticateMiddleware>();
        }
    }
}
