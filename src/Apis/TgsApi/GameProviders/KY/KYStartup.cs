using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.KY.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.GameOperations.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.KY
{
    public sealed class KYStartup : BaseStartup
    {
        public KYStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "ky";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddErrorMappers();
            services.AddScoped<IRequestAnalyzer, RequestAnalyzer>();
            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(KYGameInitializer) }, (provider, _) =>
            provider.GetRequiredService<KYGameInitializer>());
            services.AddScopedWithResolver<IGameProviderWalletAction>(new[] { typeof(KYGameProviderWalletAction) }, (provider, _) =>
            provider.GetRequiredService<KYGameProviderWalletAction>());
            services.AddScopedWithResolver<IGameHistory>(new[] { typeof(KYGameHistory) }, (provider, _) =>
            provider.GetRequiredService<KYGameHistory>());

        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/provider";
            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
               builder =>
               {

               });

        }
    }
}
