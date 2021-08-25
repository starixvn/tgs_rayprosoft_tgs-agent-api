using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Middlewares;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming
{
    public sealed class AsiaGamingStartup : BaseStartup
    {
        public AsiaGamingStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "asiagaming";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(AsiaGamingErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.AsiaGaming => provider.GetRequiredService<AsiaGamingErrorMapper>(),
                    _                       => provider.GetRequiredService<TgsErrorMapper>()
                }
            );

            services.AddScoped<AuthorizeAsiaGamingMiddleware>();
            services.AddScoped<HandleAsiaGamingExceptionMiddleware>();

            services.AddScoped<IAsiaGamingServices, AsiaGamingServices>();
            services.AddSingleton<IAsiaGamingActionModelMapper, AsiaGamingActionModelMapper>();

            services.AddScopedWithResolver<IGameHistory>(new[]
            {
                typeof(AsiaGamingGameHistory)
            }, (provider, _) =>
                provider.GetRequiredService<AsiaGamingGameHistory>());

            services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(AsiaGamingGameInitializer)
            }, (provider, _) =>
                provider.GetRequiredService<AsiaGamingGameInitializer>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/api/ag";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleAsiaGamingExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeAsiaGamingMiddleware>();
                });
        }
    }
}