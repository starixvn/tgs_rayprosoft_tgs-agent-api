using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.WGHorse.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.WGHorse
{
    public sealed class WGHorseStartup : BaseStartup
    {
        public WGHorseStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "wghorse";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers();

            services.AddTransient<IWGHorseService, WGHorseService>();

            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(WGHorseGameInitializer) }, (provider, _) =>
            provider.GetRequiredService<WGHorseGameInitializer>());

            services.AddScopedWithResolver<IGameProviderWalletAction>(new[] { typeof(WGHorseGameProviderWalletAction) }, (provider, _) =>
            provider.GetRequiredService<WGHorseGameProviderWalletAction>());

        }
    }
}
