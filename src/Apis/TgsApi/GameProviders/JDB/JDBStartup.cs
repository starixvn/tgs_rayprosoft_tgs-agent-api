using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.JDB.Middlewares;
using TGS.Apis.TgsApi.GameProviders.JDB.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.JDB
{
    public sealed class JDBStartup : BaseStartup
    {
        public JDBStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "jdb";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(JDBErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.JDB => provider.GetRequiredService<JDBErrorMapper>(),
                    _                => provider.GetRequiredService<TgsErrorMapper>()
                }
            );

            services.AddScoped<AuthorizeJDBMiddleware>();
            services.AddScoped<HandleJDBExceptionMiddleware>();

            services.AddScoped<IJDBServices, JDBServices>();
            services.AddSingleton<IJDBActionModelMapper, JDBActionModelMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(JDBGameInitializer)
            }, (provider, _) =>
                provider.GetRequiredService<JDBGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[]
            {
                typeof(JDBGameHistory)
            }, (provider, _) =>
                provider.GetRequiredService<JDBGameHistory>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/api/jdb";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleJDBExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeJDBMiddleware>();
                });
        }
    }
}