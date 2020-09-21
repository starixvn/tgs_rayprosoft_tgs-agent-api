using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Filters;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Services;
using TGS.Core.Models.Interfaces;

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
            services.AddSingleton<IErrorMapper, RedTigerErrorMapper>();
            services.AddScoped<HandleRedTigerErrorFilter>();
        }

        protected override void AddCustomFilters(MvcOptions options)
        {
            options.Filters.Add<HandleRedTigerErrorFilter>();
        }

    }
}
