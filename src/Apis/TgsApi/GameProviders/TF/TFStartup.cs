using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.TF.Middlewares;
using TGS.Apis.TgsApi.GameProviders.TF.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.TF;

public sealed class TFStartup : BaseStartup
{
    protected override string LogPrefix => "tf";

    public TFStartup(IConfiguration configuration, IWebHostEnvironment env)
                : base(configuration, env)
    {
    }

    protected override void AddCustomDependencyServices(IServiceCollection services)
    {
        services.AddErrorMappers(
            new[]
            {
                typeof(TFErrorMapper)
            },
            (provider, key) => key switch
            {
                GameProvider.TFGaming => provider.GetRequiredService<TFErrorMapper>(),
                _ => provider.GetRequiredService<TgsErrorMapper>()
            });

        services.AddScoped<AuthorizeTFMiddleware>();
        services.AddScoped<HandleTFGamingExceptionMiddleware>();
        services.AddScoped<TFDispatcherMiddleware>();

        services.AddScoped<ITFActionModelMapper, TFActionModelMapper>();
        services.AddScoped<ITFService, TFService>();

        services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(TFGameInitializer)
            },
            (provider, _) => provider.GetRequiredService<TFGameInitializer>());

        services.AddScopedWithResolver<IGameHistory>(new[]
            {
                typeof(TFGameHistory)
            },
            (provider, _) => provider.GetRequiredService<TFGameHistory>());
    }

    protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
    {
        const string Route = "/tf/api";

        app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
           builder =>
           {
               builder.UseMiddleware<HandleTFGamingExceptionMiddleware>();
           });
    }

    protected override void UseCustomMiddlewaresBeforeRouting(IApplicationBuilder app)
    {
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/tf/api", StringComparison.OrdinalIgnoreCase),
            builder =>
            {
                builder.UseMiddleware<AuthorizeTFMiddleware>();
            });
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/tf/api/transfer", StringComparison.OrdinalIgnoreCase),
            builder =>
            {
                builder.UseMiddleware<TFDispatcherMiddleware>();
            });
    }
}
