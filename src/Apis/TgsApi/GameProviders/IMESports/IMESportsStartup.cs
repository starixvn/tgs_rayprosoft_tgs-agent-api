// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.IMESports.Middlewares;
using TGS.Apis.TgsApi.GameProviders.IMESports.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.IMESports
{
    public sealed class IMESportsStartup : BaseStartup
    {
        public IMESportsStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "imesports";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[] {typeof(IMESportsErrorMapper)},
                (provider, key) => key switch
                {
                    GameProvider.IMESports => provider.GetRequiredService<IMESportsErrorMapper>(),
                    _                      => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeIMESportsMiddleware>();
            services.AddScoped<HandleIMESportsExceptionMiddleware>();

            services.AddScoped<IIMESportsServices, IMESportsServices>();
            services.AddSingleton<IIMESportsActionModelMapper, IMESportsActionModelMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[] {typeof(IMESportsGameInitializer)}, (provider, _) =>
                provider.GetRequiredService<IMESportsGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[] {typeof(IMESportsGameHistory)}, (provider, _) =>
                provider.GetRequiredService<IMESportsGameHistory>());
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/imesports";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleIMESportsExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeIMESportsMiddleware>();
                });
        }
    }
}