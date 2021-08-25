using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Filters;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Middlewares;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger
{
    public sealed class RedTigerStartup : BaseStartup
    {
        public RedTigerStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "redtiger";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(RedTigerErrorMapper)
                },
                (provider, key) =>
                {
                    return key switch
                    {
                        GameProvider.RedTiger => provider.GetRequiredService<RedTigerErrorMapper>(),
                        _                     => provider.GetRequiredService<TgsErrorMapper>()
                    };
                });

            services.AddScoped<HandleRedTigerErrorFilter>();
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
            => app.UseMiddleware<RedTigerDispatcherMiddleware>();
    }
}