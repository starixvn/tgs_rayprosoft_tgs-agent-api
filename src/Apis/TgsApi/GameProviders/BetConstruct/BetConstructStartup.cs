using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Middlewares;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct
{
    public sealed class BetConstructStartup : BaseStartup
    {
        public BetConstructStartup(IConfiguration configuration, IWebHostEnvironment env) 
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "betconstruct";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(BetConstructErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.BetConstruct => provider.GetRequiredService<BetConstructErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeBetConstructMiddleware>();
            services.AddScoped<IBetConstructService, BetConstructService>();
            services.AddScoped<IBetConstructActionModelMapper, BetConstructActionModelMapper>();

        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/bc/api";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
               builder =>
               {
                   builder.UseMiddleware<AuthorizeBetConstructMiddleware>();
               });
        }
    }
}
