using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.Commons.Services.GameProviderRecord;
using TGS.Apis.TgsApi.GameProviders.LY.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.TgsApi.GameProviders.LY
{
    public sealed class LYStartup : BaseStartup
    {
        protected override string LogPrefix => "ly";

        public LYStartup(IConfiguration configuration, IWebHostEnvironment env) :
                    base(configuration, env)
        {
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddErrorMappers();
            services.AddTransient<IRequestAnalyzer, RequestAnalyzer>();
            services.AddTransient<IGameProviderRecordService, GameProviderRecordService>();
            services.AddTransient<ILYService, LYService>();
            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(LYGameInitializer) }, (provider, _) =>
            provider.GetRequiredService<LYGameInitializer>());
            services.AddScopedWithResolver<IGameProviderWalletAction>(new[] { typeof(LYGameProviderWalletAction) }, (provider, _) =>
            provider.GetRequiredService<LYGameProviderWalletAction>());
            services.AddScopedWithResolver<IGameHistory>(new[] { typeof(LYGameHistory) }, (provider, _) =>
            provider.GetRequiredService<LYGameHistory>());
        }
    }
}
