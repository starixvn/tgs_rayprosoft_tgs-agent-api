using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Filters;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Middlewares;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger
{
    public sealed class RedTigerStartup : BaseStartup
    {
        protected override string LogPrefix => "redtiger";

        public RedTigerStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
            
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {            
            services.AddErrorMappers(
                addCustomErrorMappers: () => 
                { 
                    services.AddSingleton<RedTigerErrorMapper>();
                }, 
                get: provider => key => 
                {
                    return key switch    
                    {
                        Consts.GameProvider.RedTiger => provider.GetRequiredService<RedTigerErrorMapper>(),
                        _ => provider.GetRequiredService<TgsErrorMapper>()                            
                    }; 
                });

            services.AddScoped<HandleRedTigerErrorFilter>();             
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            app.UseMiddleware<RedTigerDispatcherMiddleware>();
        }
    }
}
