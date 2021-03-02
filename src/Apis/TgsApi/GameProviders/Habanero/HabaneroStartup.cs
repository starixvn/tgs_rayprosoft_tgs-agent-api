using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Habanero.Handlers;
using TGS.Apis.TgsApi.GameProviders.Habanero.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Habanero
{
    public sealed class HabaneroStartup : BaseStartup
    {
        public HabaneroStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "habanero";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[] {typeof(HabaneroErrorMapper)},
                (provider, key) => key switch
                {
                    GameProvider.Habanero => provider.GetRequiredService<HabaneroErrorMapper>(),
                    _                     => provider.GetRequiredService<TgsErrorMapper>()
                }
            );

            services.AddScoped<AuthorizeHabaneroMiddleware>();
            services.AddScoped<HandleHabaneroExceptionMiddleware>();

            //services.AddSingleton<IHabaneroGameControllerMapper, HabaneroGameControllerMapper>();
            services.AddTransient<IHabaneroGameControllerMapper, HabaneroGameControllerMapper>();
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/provider";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleHabaneroExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeHabaneroMiddleware>();
                });
        }

        protected override Type[] AddCustomMediatrHandlers() => new[]
        {
            typeof(AuthorizePlayerHandler)
        };
    }
}