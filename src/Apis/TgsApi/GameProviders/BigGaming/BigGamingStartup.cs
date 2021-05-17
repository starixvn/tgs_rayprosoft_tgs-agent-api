using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming
{
    public sealed class BigGamingStartup : BaseStartup
    {
        public BigGamingStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "biggaming";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers();
            services.AddTransient<IBigGamingGamePlayerCreate, BigGamingGamePlayerCreate>();
            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(BigGamingGameInitializer) }, (provider, _) =>
            provider.GetRequiredService<BigGamingGameInitializer>());
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            app.UseBGMethodMiddleware();
        }

    }

}
