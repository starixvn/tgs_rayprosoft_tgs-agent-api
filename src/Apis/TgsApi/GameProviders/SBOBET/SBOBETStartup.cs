// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;

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
                new[] {typeof(SBOBETErrorMapper)},
                (provider, key) => key switch
                {
                    GameProvider.SBOBET => provider.GetRequiredService<SBOBETErrorMapper>(),
                    _                   => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeSBOBETMiddleware>();
            services.AddScoped<HandleSBOBETExceptionMiddleware>();

            services.AddScoped<ISBOBETServices, SBOBETServices>();
            services.AddSingleton<ISBOBETGameControllerMapper, SBOBETGameControllerMapper>();

            services.AddScopedWithResolver<IGameInitializer>(new[] {typeof(SBOBETGameInitializer)}, (provider, _) =>
                provider.GetRequiredService<SBOBETGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[] {typeof(SBOBETGameHistory)}, (provider, _) =>
                provider.GetRequiredService<SBOBETGameHistory>());

            services.AddScopedWithResolver<IPlayerAction>(new[] {typeof(SBOBETPlayerAction)}, (provider, _) =>
                provider.GetRequiredService<SBOBETPlayerAction>());
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

        protected override Type[] AddCustomMediatrHandlers() => new[]
        {
            typeof(GetBalanceHandler), typeof(PlaceBetHandler),
            typeof(SettleBetHandler), typeof(CancelBetHandler),
            typeof(RollbackHandler), typeof(GetBetStatusHandler),
            typeof(BatchCancelHandler)
        };
    }
}