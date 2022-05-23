using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tgs.MessageQueue.MongoDB.Configuration;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Middlewares;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using static TGS.Core.Models.Consts;
using MongoMessageQueue = MongoDB.Messaging.MessageQueue;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct
{
    public sealed class BetConstructStartup : BaseStartup
    {
        protected override string LogPrefix => "betconstruct";

        public BetConstructStartup(IConfiguration configuration, IWebHostEnvironment env)
                    : base(configuration, env)
        {
        }

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(
                new[]
                {
                    typeof(BetConstructErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.BetConstruct => provider.GetRequiredService<BetConstructErrorMapper>(),
                    _ => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeBetConstructMiddleware>();
            services.AddScoped<IBetConstructService, BetConstructService>();
            services.AddScoped<IBetConstructActionModelMapper, BetConstructActionModelMapper>();
            services.AddScopedWithResolver<IGameInitializer>(new[]
                {
                    typeof(BetConstructGameInitializer)
                },
                (provider, _) => provider.GetRequiredService<BetConstructGameInitializer>());

            services.AddScopedWithResolver<IGameHistory>(new[]
                {
                    typeof(BCGameHistory)
                },
                (provider, _) => provider.GetRequiredService<BCGameHistory>());

            services.AddMessageQueue()                    
                    .AddMongoMessageQueue(config =>
                    {
                        _ = config
                        .DefineMessageQueue(
                            "gp.gamerecord.database",
                            sp => new MongoMessageQueueBuilderOptions
                            {
                                MessageQueue = MongoMessageQueue.Default,
                                ConnectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("MongoDBMessagingConnectionString")
                            });
                    })
                    .ClearAllExchanges()                    
                    .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.detail.bc", "gp.gamerecord.database", "tgs.gp.gamerecord.database.detail.bc");
        }

        protected override void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
            const string Route = "/bc/api";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
               builder =>
               {
                   builder.UseMiddleware<AuthorizeBetConstructMiddleware>();
               });
        }
    }
}
