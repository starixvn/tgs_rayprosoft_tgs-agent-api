using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.Commons.Services.GameProviderDepositWithdrawal;
using TGS.Apis.TgsApi.Commons.Services.GameProviderRecord;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;
namespace TGS.Apis.TgsApi.GameProviders.BigGaming
{
    public sealed class BigGamingStartup : BaseStartup
    {
        public BigGamingStartup(IConfiguration configuration, IWebHostEnvironment env) :
            base(configuration, env)
        {
        }

        protected override string LogPrefix => "biggaming";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
              new[] { typeof(BigGamingErrorMapper) },
              (provider, key) => key switch
              {
                  GameProvider.BigGaming => provider.GetRequiredService<BigGamingErrorMapper>(),
                  _ => provider.GetRequiredService<TgsErrorMapper>()
              });

            services.AddScoped<AuthorizeBigGamingMiddleware>();
            services.AddScoped<HandleBigGamingExceptionMiddleware>();
            services.AddTransient<IGameProviderRecordService, GameProviderRecordService>();
            services.AddScoped<IBigGamingService, BigGamingService>();
            services.AddScoped<IGameProviderDepositWithdrawalService, GameProviderDepositWithdrawalService>();
            services.AddScoped<IBigGamingGPGameHistoryService, BigGamingGPGameHistoryService>();
            services.AddScoped<IBigGamingModelActionMapper, BigGamingActionModelMapper>();
            services.AddScoped<IBigGamingGameHistoryService, BigGamingGameHistoryService>();
            services.AddTransient<IBigGamingGamePlayerCreate, BigGamingGamePlayerCreate>();
            services.AddScopedWithResolver<IGameInitializer>(new[] { typeof(BigGamingGameInitializer) }, (provider, _) =>
            provider.GetRequiredService<BigGamingGameInitializer>());
            services.AddScopedWithResolver<IGameHistory>(new[] { typeof(BigGamingGameHistory) }, (provider, _) =>
            provider.GetRequiredService<BigGamingGameHistory>());
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/bg";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleBigGamingExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeBigGamingMiddleware>();

                });
        }

    }

}
