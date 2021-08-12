using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.WGSports.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.WGSports
{
    public sealed class WGSportsStartup : BaseStartup
    {
        public WGSportsStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "wg";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers();

            services.AddTransient<IWGSportsService, WGSportsService>();

            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(WGSportsGameInitializer) }, (provider, _) =>
              provider.GetRequiredService<WGSportsGameInitializer>());

        }
    }
}
