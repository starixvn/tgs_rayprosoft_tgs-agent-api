using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Services.GameOperations.Calculators;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino
{
    public sealed class WMCasinoStartup : BaseStartup
    {
        public WMCasinoStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        protected override string LogPrefix => "wmcasino";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(new[]
                {
                    typeof(WMCasinoErrorMapper)
                },
                (provider, key) => key switch
                {
                    Consts.GameProvider.WMCasino => provider.GetRequiredService<WMCasinoErrorMapper>(),
                    _                            => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<WMCasinoDispatcherMiddleware>();
            services.AddScoped<AuthorizeWMCasinoMiddleware>();
            services.AddScoped<HandleWMCasinoExceptionMiddleware>();

            services.AddScoped<IWMCasinoServices, WMCasinoServices>();
            services.AddScoped<IWMCasinoBetActionServices, WMCasinoBetActionServices>();
            services.AddScoped<IWMCasinoAgentMapper, WMCasinoAgentMapper>();
            services.AddScoped<IWMCasinoActionModelMapper, WMCasinoActionModelMapper>();
            services.AddScoped<IWMCasinoUtility, WMCasinoUtility>();
            services.AddScoped<IWMCasinoPlayerServices, WMCasinoPlayerServices>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(WMCasinoGameInitializer)
                },
                (provider, _) => provider.GetRequiredService<WMCasinoGameInitializer>());

            //services.AddScopedWithResolver<IGameHistory>(new[]
            //    {
            //        typeof(SabaGameHistory)
            //    },
            //    (provider, _) => provider.GetRequiredService<SabaGameHistory>());

            services.AddSingleton<IBetValuesCalculator, WMCasinoBetValuesCalculator>();
        }

        protected override void UseCustomMiddlewaresBeforeRouting(IApplicationBuilder app)
        {
            const string route = "/api/wm/entrance";

            app.UseWhen(
                context =>
                    context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase) &&
                    HttpMethods.IsPost(context.Request.Method),
                builder =>
                {
                    builder.UseMiddleware<HandleWMCasinoExceptionMiddleware>();
                    builder.UseMiddleware<WMCasinoDispatcherMiddleware>();
                });
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string route = "/api/wm";

            app.UseWhen(
                context =>
                {
                    if (context.Request.ContentType != MediaTypeNames.Application.Json)
                    {
                        return false;
                    }

                    return context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase) &&
                        !context.Request.Path.StartsWithSegments("/gamerecord",
                            StringComparison.OrdinalIgnoreCase);
                },
                builder =>
                {
                    builder.UseMiddleware<AuthorizeWMCasinoMiddleware>();
                });
        }
    }
}