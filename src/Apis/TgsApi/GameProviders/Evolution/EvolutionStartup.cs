using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Evolution.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Evolution.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Evolution
{
    public sealed class EvolutionStartup : BaseStartup
    {
        public EvolutionStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "evo";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
              new[] { typeof(EvolutionErrorMapper) },
              (provider, key) => key switch
              {
                  GameProvider.Evolution => provider.GetRequiredService<EvolutionErrorMapper>(),
                  _ => provider.GetRequiredService<TgsErrorMapper>()
              });

            services.AddScoped<HandleEvolutionExceptionMiddleware>();
            services.AddScoped<AuthorizeEvolutionMiddleware>();

            services.AddScoped<IEvolutionServices, EvolutionServices>();
            services.AddScoped<IEvolutionActionModelMapper, EvolutionActionModelMapper>();
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/evo/api/";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleEvolutionExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeEvolutionMiddleware>();
                });
        }
    }
}
