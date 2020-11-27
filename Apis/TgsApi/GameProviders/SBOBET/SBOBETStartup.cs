// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET
{
    public sealed class SBOBETStartup : BaseStartup
    {
        public SBOBETStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "sbo";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                () => services.AddSingleton<SBOBETErrorMapper>(),
                provider => key =>
                {
                    return key switch
                    {
                        Consts.GameProvider.SBOBET => provider.GetRequiredService<SBOBETErrorMapper>(),
                        _                          => provider.GetRequiredService<TgsErrorMapper>()
                    };
                });

            services.AddScoped<AuthorizeSBOBETMiddleware>();
            services.AddScoped<HandleSBOBETExceptionMiddleware>();

            services.AddScoped<ISBOBETServices, SBOBETServices>();
            services.AddSingleton<ISBOBETGameControllerMapper, SBOBETGameControllerMapper>();
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/provider";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleSBOBETExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeSBOBETMiddleware>();
                });
        }
    }
}