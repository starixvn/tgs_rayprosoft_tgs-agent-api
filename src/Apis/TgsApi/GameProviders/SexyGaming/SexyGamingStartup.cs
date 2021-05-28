using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.SexyGaming.Middlewares;
using TGS.Apis.TgsApi.GameProviders.SexyGaming.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming
{
    public sealed class SexyGamingStartup : BaseStartup
    {
        public SexyGamingStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "sexygaming";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
            new[] { typeof(SexyGamingErrorMapper) },
            (provider, key) => key switch
            {
                GameProvider.SexyGaming => provider.GetRequiredService<SexyGamingErrorMapper>(),
                _ => provider.GetRequiredService<TgsErrorMapper>()
            });

            services.AddScoped<HandleSexyGamingExceptionMiddleware>();
            services.AddScoped<AuthorizeSexyGamingMiddleware>();


        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/sg";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleSexyGamingExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeSexyGamingMiddleware>();
                });
        }
    }
}
