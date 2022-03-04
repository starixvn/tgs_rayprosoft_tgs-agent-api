using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Skywind.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Skywind
{
    public sealed class SkywindStartup : BaseStartup
    {
        protected override string LogPrefix => "skywind";

        public SkywindStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            _ = services.AddErrorMappers(
                new[]
                {
                    typeof(SkywindErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.Skywind => provider.GetRequiredService<SkywindErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                }
            );

            _ = services.AddScoped<AuthorizeSkywindMiddleware>();
            _ = services.AddScoped<HandleSkywindExceptionMiddleware>();
            _ = services.AddScoped<ISkywindService, SkywindService>();
            _ = services.AddScoped<ISkywindActionModelMapper, SkywindActionModelMapper>();

            _ = services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(SkywindGameInitializer)
            }, (provider, _) =>
                provider.GetRequiredService<SkywindGameInitializer>());

            _ = services.AddScopedWithResolver<IGameHistory>(new[]
             {
                typeof(SkywindGameHistory)
            }, (provider, _) =>
                provider.GetRequiredService<SkywindGameHistory>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/sw/api";

            _ = app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    _ = builder.UseMiddleware<HandleSkywindExceptionMiddleware>();
                    _ = builder.UseMiddleware<AuthorizeSkywindMiddleware>();
                });
        }
    }
}
