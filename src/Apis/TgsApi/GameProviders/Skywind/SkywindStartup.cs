using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Skywind
{
    public sealed class SkywindStartup : BaseStartup
    {
        public SkywindStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "skywind";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
            new[] { typeof(SkywindErrorMapper) },
            (provider, key) => key switch
            {
                GameProvider.Skywind => provider.GetRequiredService<SkywindErrorMapper>(),
                _ => provider.GetRequiredService<TgsErrorMapper>()
            });

            services.AddScoped<ISkywindServices, SkywindServices>();
            services.AddScoped<ISkywindActionModelMapper, SkywindActionModelMapper>();


        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/sw";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                });
        }
    }
}
