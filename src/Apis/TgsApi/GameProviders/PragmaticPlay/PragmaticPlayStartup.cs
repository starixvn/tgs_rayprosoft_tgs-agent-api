using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Core.Infrastructures.ErrorMappers;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay
{
    public sealed class PragmaticPlayStartup : BaseStartup
    {
        public PragmaticPlayStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "pragmaticplay";

        protected override void AddCustomDependencyServices(IServiceCollection services) => services.AddErrorMappers();

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
        }
    }
}