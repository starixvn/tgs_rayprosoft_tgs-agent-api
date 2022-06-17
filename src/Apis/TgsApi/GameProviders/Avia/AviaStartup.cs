using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Avia.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.Avia;

public sealed class AviaStartup : BaseStartup
{
    protected override string LogPrefix => "avia";

    public AviaStartup(IConfiguration configuration, IWebHostEnvironment env)
        : base(configuration, env)
    {
    }

    protected override void AddCustomDependencyServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddErrorMappers();
        services.AddTransient<IAviaService, AviaService>();
        services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(AviaGameInitializer) }, (provider, _) =>
        provider.GetRequiredService<AviaGameInitializer>());
        services.AddScopedWithResolver<IGameHistory>(new[] { typeof(AviaGameHistory) }, (provider, _) =>
        provider.GetRequiredService<AviaGameHistory>());
    }
}
