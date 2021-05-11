using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.KY.Services;
using TGS.Apis.TgsApi.GameProviders.VR.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.GameOperations.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.VR
{
    public sealed class VRStartup : BaseStartup
    {
        public VRStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "vr";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers();

            services.AddTransient<IVRGamePlayerCreate, VRGamePlayerCreate>();

            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(VRGameInitializer) }, (provider, _) =>
              provider.GetRequiredService<VRGameInitializer>());

            services.AddScopedWithResolver<IGameProviderWalletAction>(new[] { typeof(VRGameProviderWalletAction) }, (provider, _) =>
            provider.GetRequiredService<VRGameProviderWalletAction>());

            services.AddScopedWithResolver<IGameHistory>(new[] { typeof(VRGameHistory) }, (provider, _) =>
            provider.GetRequiredService<VRGameHistory>());
        }
    }
}
