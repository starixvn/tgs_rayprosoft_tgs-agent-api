using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;

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
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
        }
    }
}
