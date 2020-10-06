using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Filters;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET
{
    public sealed class SBOBETStartup : BaseStartup
    {
        protected override string LogPrefix => "sbo";

        public SBOBETStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                addCustom: () => services.AddSingleton<SBOErrorMapper>(),
                get: provider => key =>
                {
                    return key switch
                    {
                        Consts.GameProvider.SBOBET => provider.GetRequiredService<SBOErrorMapper>(),
                        _ => provider.GetRequiredService<TgsErrorMapper>()
                    };
                });

            services.AddScoped<HandleSBOErrorFilter>();
        }
    }
}