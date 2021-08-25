using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Habanero.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
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
                new[]
                {
                    typeof(HabaneroErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.Habanero => provider.GetRequiredService<HabaneroErrorMapper>(),
                    _                     => provider.GetRequiredService<TgsErrorMapper>()
                }
            );

            services.AddScoped<AuthorizeHabaneroMiddleware>();
            services.AddScoped<HandleHabaneroExceptionMiddleware>();

            services.AddScoped<IHabaneroServices, HabaneroServices>();
            services.AddSingleton<IHabaneroActionModelMapper, HabaneroActionModelMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(HabaneroGameInitializer)
            }, (provider, _) =>
                provider.GetRequiredService<HabaneroGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[]
            {
                typeof(HabaneroGameHistory)
            }, (provider, _) =>
                provider.GetRequiredService<HabaneroGameHistory>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/api/haba";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleHabaneroExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeHabaneroMiddleware>();
                });
        }
    }
}