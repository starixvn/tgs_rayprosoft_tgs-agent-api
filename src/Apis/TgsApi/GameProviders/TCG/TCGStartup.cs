using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.Commons.Services.GameProviderRecord;
using TGS.Apis.TgsApi.GameProviders.VR.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.TCG
{
    public sealed class TCGStartup : BaseStartup
    {
        public TCGStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "tcg";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers();
        }
    }
}
