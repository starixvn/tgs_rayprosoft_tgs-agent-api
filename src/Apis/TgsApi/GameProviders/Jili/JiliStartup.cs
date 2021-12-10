using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Jili.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Jili
{
    public class JiliStartup : BaseStartup
    {
        protected override string LogPrefix => "jili";

        public JiliStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(JiliErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.Jili => provider.GetRequiredService<JiliErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddSingleton<IJiliMerchantMapper, JiliMerchantMapper>();
            services.AddSingleton<IJiliPlayerService, JiliPlayerService>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(JiliGameInitializer)
            },
                (provider, _)
                    => provider.GetRequiredService<JiliGameInitializer>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
        }
    }
}
