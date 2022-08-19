using Microsoft.Extensions.DependencyInjection;
using Tgs.MarketingReport.Client;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Cache.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackOfficeCache(this IServiceCollection services)
        => services
            .Decorate<IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics>, CachedAgentPlayerDepositQueryService>()
            .Decorate<IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem>, CachedPlayerPaymentStatisticsDetailQueryService>()
            .Decorate<IQueryService<AgentMarketingReportQuery, DailyAggregateInfo>, CachedAgentMarketingReportQueryService>()
            .Decorate<IQueryService<AgentPlayerGamingDetailQuery, PlayerGamingDetail>, CachedAgentPlayerGamingDetailQueryService>()
            .Decorate<IQueryService<AgentPlayerRegisterDetailQuery, PlayerRegisterDetail>, CachedAgentPlayerRegisterDetailQueryService>();
}
