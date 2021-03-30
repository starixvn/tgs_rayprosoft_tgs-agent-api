using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Middlewares;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay
{
    public sealed class PragmaticPlayStartup : BaseStartup
    {
        public PragmaticPlayStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "pragmaticplay";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[] {typeof(PragmaticPlayErrorMapper)},
                (provider, key) => key switch
                {
                    GameProvider.PragmaticPlay => provider.GetRequiredService<PragmaticPlayErrorMapper>(),
                    _                          => provider.GetRequiredService<TgsErrorMapper>()
                }
            );

            services.AddScoped<AuthorizePragmaticPlayMiddleware>();
            services.AddScoped<HandlePragmaticPlayExceptionMiddleware>();

            services.AddScoped<IPragmaticPlayServices, PragmaticPlayServices>();

            services.AddSingleton<IPragmaticPlayActionModelMapper, PragmaticPlayActionModelMapper>();
            services.AddSingleton<IPragmaticPlayHashProvider, PragmaticPlayHashProvider>();
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/provider";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandlePragmaticPlayExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizePragmaticPlayMiddleware>();
                });
        }
    }
}