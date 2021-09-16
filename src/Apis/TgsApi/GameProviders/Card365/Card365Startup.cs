using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.Card365.Middlewares;
using TGS.Apis.TgsApi.GameProviders.Card365.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Services.GameOperations.Calculators;

namespace TGS.Apis.TgsApi.GameProviders.Card365
{
    public class Card365Startup : BaseStartup
    {
        public Card365Startup(IConfiguration configuration, IWebHostEnvironment env)
            : base(configuration, env)
        {
        }

        protected override string LogPrefix => "365";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(new[]
                {
                    typeof(Card365ErrorMapper)
                },
                (provider, key) => key switch
                {
                    Consts.GameProvider.Card365 => provider.GetRequiredService<Card365ErrorMapper>(),
                    _                           => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<Card365DispatcherMiddleware>();
            services.AddScoped<AuthorizeCard365Middleware>();
            services.AddScoped<HandleCard365ExceptionMiddleware>();

            services.AddScoped<ICard365ActionModelMapper, Card365ActionModelMapper>();
            services.AddScoped<ICard365BetActionServices, Card365BetActionServices>();
            services.AddScoped<ICard365Services, Card365Services>();
            services.AddScoped<ICard365AgentMapper, Card365AgentMapper>();

            services.AddSingleton<IBetValuesCalculator, Card365BetValuesCalculator>();

            services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(Card365GameInitializer)
                },
                (provider, _) => provider.GetRequiredService<Card365GameInitializer>());

            //services.AddScopedWithResolver<IGameHistory>(new[]
            //    {
            //        typeof(WMCasinoGameHistory)
            //    },
            //    (provider, _) => provider.GetRequiredService<WMCasinoGameHistory>());
        }

        protected override void UseCustomMiddlewaresBeforeRouting(IApplicationBuilder app)
        {
            const string route = "/api/365/entrance";

            app.UseWhen(
                context =>
                    context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase) &&
                    HttpMethods.IsPost(context.Request.Method),
                builder =>
                {
                    builder.UseMiddleware<HandleCard365ExceptionMiddleware>();
                    builder.UseMiddleware<Card365DispatcherMiddleware>();
                });
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string route = "/api/365";

            app.UseWhen(
                context =>
                    context.Request.ContentType == MediaTypeNames.Application.Json &&
                    context.Request.Path.StartsWithSegments(route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<AuthorizeCard365Middleware>();
                });
        }
    }
}