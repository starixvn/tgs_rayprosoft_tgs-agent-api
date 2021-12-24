using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.TF.Middlewares;
using TGS.Apis.TgsApi.GameProviders.TF.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.TF
{
    public sealed class TFStartup : BaseStartup
    {
        public TFStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "tf";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(TFErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.TFGaming => provider.GetRequiredService<TFErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeTFMiddleware>();
            services.AddScoped<ITFService, TFService>();
            services.AddScoped<ITFActionModelMapper, TFActionModelMapper>();
            services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(TFGameInitializer)
                },
                (provider, _) => provider.GetRequiredService<TFGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[]
                {
                    typeof(TFGameHistory)
                },
                (provider, _) => provider.GetRequiredService<TFGameHistory>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/tf/api";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
               builder =>
               {
                   builder.UseMiddleware<AuthorizeTFMiddleware>();
               });
        }
    }
}
