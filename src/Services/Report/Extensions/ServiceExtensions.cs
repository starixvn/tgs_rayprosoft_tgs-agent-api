using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddReportServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<ITgsPlayerService, TgsPlayerService>();
            services.AddTransient<ITgsRoundService, TgsRoundService>();
            services.AddTransient<ITgsAgentService, TgsAgentService>();
            services.AddTransient<ITgsBrandService, TgsBrandService>();
            services.AddTransient<ITgsPromotionService, TgsPromotionService>();
            services.AddTransient<IAgentDividendService, AgentDividendService>();
            services.AddTransient<IPlayerAggregateService, PlayerAggregateService>();
            services.AddTransient<IPlayerGameAggregateService, PlayerGameAggregateService>();
            services.AddTransient<IRoundAggregateService, RoundAggregateService>();
            services.AddTransient<IPlayerRebateService, PlayerRebateService>();

            services.AddPlayerBucketRepositoriesServices();
            services.AddWallet()
                .AddTgsWalletRepositories()
                .AddTgsAgentWalletRepositories();

            return services;
        }
    }
}