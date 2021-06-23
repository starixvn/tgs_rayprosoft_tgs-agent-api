using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;
using TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares;
using TGS.Apis.TgsApi.GameProviders.AllBet.Services;
using TGS.Core.Infrastructures.ErrorMappers;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.AllBet
{
    public class AllBetStartup : BaseStartup
    {
        public AllBetStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        protected override string LogPrefix => "allbet";

        protected override void AddCustomDependencyServices(IServiceCollection services)
        {
            services.AddErrorMappers(new[]
                {
                    typeof(AllBetErrorMapper)
                },
                (provider, key) => key switch
                {
                    GameProvider.AllBet => provider.GetRequiredService<AllBetErrorMapper>(),
                    _                   => provider.GetRequiredService<TgsErrorMapper>()
                });

            services.AddScoped<AuthorizeAllBetMiddleware>();
            services.AddScoped<HandleAllBetExceptionMiddleware>();

            services.AddScoped<IAllBetActionModelMapper, AllBetActionModelMapper>();
            services.AddScoped<IAllBetServices, AllBetServices>();
        }

        protected override void UseCustomMiddlewares(IApplicationBuilder app)
        {
            const string Route = "/api/ab";

            app.UseWhen(context => context.Request.Path.StartsWithSegments(Route, StringComparison.OrdinalIgnoreCase),
                builder =>
                {
                    builder.UseMiddleware<HandleAllBetExceptionMiddleware>();
                    builder.UseMiddleware<AuthorizeAllBetMiddleware>();
                });
        }
    }
}