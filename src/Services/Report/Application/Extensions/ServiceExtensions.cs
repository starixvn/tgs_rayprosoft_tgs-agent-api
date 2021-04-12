using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Transactions;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Extensions
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
            services.AddTransient<ITgsTransactionService, TgsTransactionService>();
            services.AddTransient<IAgentDividendService, AgentDividendService>();
            services.AddTransient<IPlayerAggregateService, PlayerAggregateService>();
            services.AddTransient<IPlayerGameTypeAggregateService, PlayerGameTypeAggregateService>();
            services.AddTransient<IRoundAggregateService, RoundAggregateService>();
            services.AddTransient<IPlayerRebateService, PlayerRebateService>();
            services.AddTransient<ITransactionManager, TransactionManager>();

            return services;
        }
    }
}