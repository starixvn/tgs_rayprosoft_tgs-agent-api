using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Saba.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

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
            services.AddErrorMappers(
                new[] {typeof(SabaErrorMapper)},
                (provider, key) => key switch
                {
                    GameProvider.Saba => provider.GetRequiredService<SabaErrorMapper>(),
                    _                 => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeSabaMiddleware>();
            services.AddScoped<HandleSabaExceptionMiddleware>();

            services.AddScoped<ISabaServices, SabaServices>();
            services.AddScoped<ISabaActionModelMapper, SabaActionModelMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[] {typeof(SabaGameInitializer)},
                (provider, _) => provider.GetRequiredService<SabaGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[] {typeof(SabaGameHistory)},
                (provider, _) => provider.GetRequiredService<SabaGameHistory>());
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/saba";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleSabaExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeSabaMiddleware>();
                });
        }
    }
}