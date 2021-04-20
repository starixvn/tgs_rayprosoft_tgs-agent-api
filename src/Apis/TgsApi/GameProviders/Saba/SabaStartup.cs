using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Saba.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Core.Infrastructures.ErrorMappers;

namespace TGS.Apis.TgsApi.GameProviders.Saba
{
    public sealed class SabaStartup : BaseStartup
    {
        public SabaStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "saba";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers();

            services.AddScoped<AuthorizeSabaMiddleware>();

            services.AddScoped<ISabaServices, SabaServices>();
            services.AddScoped<ISabaActionModelMapper, SabaActionModelMapper>();
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/saba";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    //builder.UseMiddleware<HandlePragmaticPlayExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeSabaMiddleware>();
                });
        }
    }
}