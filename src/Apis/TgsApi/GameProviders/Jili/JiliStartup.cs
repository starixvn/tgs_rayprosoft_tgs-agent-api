using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Jili.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Jili.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.Jili
{
    public class JiliStartup : BaseStartup
    {
        protected override string LogPrefix => "jili";

        public JiliStartup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(JiliErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.Jili => provider.GetRequiredService<JiliErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddSingleton<IJiliMerchantMapper, JiliMerchantMapper>();
            services.AddSingleton<IJiliPlayerService, JiliPlayerService>();
            services.AddSingleton<IJiliActionModelMapper, JiliActionModelMapper>();

            services.AddScoped<IJiliService, JiliService>();

            services.AddScoped<HandleJiliExceptionMiddleware>();
            services.AddScoped<AuthorizeJiliMiddleware>();

            services.AddTransient<IJiliDataStore>(sp
                => ActivatorUtilities.CreateInstance<JiliRedisDataStore>(
                    sp,
                    sp.GetRequiredService<IRedisConnectionFactory>().GetConnection().GetDatabase()));

            services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(JiliGameInitializer)
            },
                (provider, _)
                    => provider.GetRequiredService<JiliGameInitializer>());
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/api/jili";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleJiliExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeJiliMiddleware>();
                });
        }
    }
}
