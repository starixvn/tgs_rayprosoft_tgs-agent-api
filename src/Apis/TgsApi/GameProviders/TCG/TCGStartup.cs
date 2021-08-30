using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.TCG.Middlewares;
using TGS.Apis.TgsApi.GameProviders.TCG.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

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
            services.AddErrorMappers(
               new[]
               {
                    typeof(TCGErrorMapper)
               },
               (provider, key) => key switch
               {
                   GameProvider.TCG => provider.GetRequiredService<TCGErrorMapper>(),
                   _ => provider.GetRequiredService<TgsErrorMapper>()
               }
               );
            services.AddScoped<AuthorizeTCGMiddleware>();
            services.AddScoped<HandleTCGExceptionMiddleware>();
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/api/tcg"; 

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleTCGExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeTCGMiddleware>();
                });
        }
    }
}
