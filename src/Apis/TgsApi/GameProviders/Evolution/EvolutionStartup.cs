using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Evolution.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Evolution.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Evolution;

public sealed class EvolutionStartup : BaseStartup
{
    protected override string LogPrefix => "evo";

    public EvolutionStartup(IConfiguration configuration, IWebHostEnvironment env) :
                base(configuration, env)
    {
    }

    protected override void AddCustomDependencyServices(IServiceCollection services)
    {
        services.AddErrorMappers(
            new[]
            {
                typeof(EvolutionErrorMapper)
            },
            (provider, key) => key switch
            {
                GameProvider.Evolution => provider.GetRequiredService<EvolutionErrorMapper>(),
                _ => provider.GetRequiredService<TgsErrorMapper>()
            });
        services.AddHttpContextAccessor();
        services.AddScoped<HandleEvolutionExceptionMiddleware>();
        services.AddScoped<AuthorizeEvolutionMiddleware>();

        services.AddScoped<IEvolutionService, EvolutionService>();
        services.AddScoped<IEvolutionActionModelMapper, EvolutionActionModelMapper>();
        services.AddScoped<IEvolutionCacheService, EvolutionCacheService>(
            sp => ActivatorUtilities.CreateInstance<EvolutionCacheService>(sp, sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase()));
        services.AddScoped<IEvolutionGameHistoryService, EvolutionGameHistoryService>();
        services.AddScoped<EvolutionExternalTransactionService>();

        services.AddScopedWithResolver<IGameInitializer>(new[]
        {
            typeof(EvolutionGameInitializer)
        }, (provider, _) =>
            provider.GetRequiredService<EvolutionGameInitializer>());
        services.AddScopedWithResolver<IGameHistory>(new[]
        {
            typeof(EvolutionGameHistory)
        }, (provider, _) =>
            provider.GetRequiredService<EvolutionGameHistory>());
    }

    protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
    {
        const string Route = "/evo/api";

        app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
            builder =>
            {
                builder.UseMiddleware<HandleEvolutionExceptionMiddleware>();
                builder.UseMiddleware<AuthorizeEvolutionMiddleware>();
            });
    }
}
