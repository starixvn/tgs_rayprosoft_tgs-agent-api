using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;

namespace TGS.Apis.TgsApi.GameProviders.Saba
{
    public sealed class SabaStartup : BaseStartup
    {
        public SabaStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "saba";

        protected override void AddCustomDependencyServices(IServiceCollection services) =>
            services.AddScoped<ISabaActionModelMapper, SabaActionModelMapper>();
    }
}