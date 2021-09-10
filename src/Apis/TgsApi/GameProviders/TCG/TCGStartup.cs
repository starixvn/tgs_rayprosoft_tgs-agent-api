using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.TCG.Middlewares;
using TGS.Apis.TgsApi.GameProviders.TCG.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Shared.Game.ApiClients.TCG;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.TCG
{
    public sealed class TCGStartup : BaseStartup
    {
        public TCGStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "tcg";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
               new[]
               {
                    typeof(TCGErrorMapper)
               },
               (provider, key) => key switch
               {
                   GameProvider.TCG => provider.GetRequiredService<TCGErrorMapper>(),
                   _ => provider.GetRequiredService<TgsErrorMapper>()
               }
               );

            services.AddGameProviderTCGFtpClientRepository((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                config.GetSection("TCGFtpApi").Bind(options);
            });

            services.AddTransient<ITCGGPGameHistoryService, TCGGPGameHistoryService>();

            services.AddScoped<AuthorizeTCGMiddleware>();
            services.AddScoped<HandleTCGExceptionMiddleware>();
            services.AddScoped<DispatcherTCGMiddleware>();
            services.AddScoped<ITCGService, TCGService>();
            services.AddScoped<ITCGUtility, TCGUtility>();
            services.AddScoped<ITCGActionModelMapper, TCGActionModelMapper>();
            services.AddScopedWithResolver<IGameInitializer>(new[]
            {
                typeof(TCGGameInitializer)
            }, (provider, _) =>
                provider.GetRequiredService<TCGGameInitializer>());

        }

        protected override void UseCustomMiddlewaresBeforeRouting(IApplicationBuilder app)
        {
            const string route = "/api/tcg/entrance";

            app.UseWhen(
                context =>
                    context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase) &&
                    HttpMethods.IsPost(context.Request.Method),
                builder =>
                {
                    builder.UseMiddleware<HandleTCGExceptionMiddleware>();
                    builder.UseMiddleware<DispatcherTCGMiddleware>();
                });
        }


        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/api/tcg";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<AuthorizeTCGMiddleware>();
                });
        }
    }
}
