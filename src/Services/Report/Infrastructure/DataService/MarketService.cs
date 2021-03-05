using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.DataServices.Report.Models;
using TGS.Services.Report.Application.Models.Market;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IMarketService
    {
        Task<Market> GetMarketAsync(Expression<Func<Market, bool>> predicate,
            Func<IQueryable<Market>, IQueryable<Market>> preQuery = null);

        IQueryable<Market> GetMarketsAsync(Func<IQueryable<Market>, IQueryable<Market>> preQuery = null,
            bool tracking = false);

        Task CreateMarketsAsync(MarketRequest[] request);
        Task DeleteMarketsAsync(Market[] markets);
        Task<bool> ExistsAsync(Expression<Func<Market, bool>> predicate);
    }

    public class MarketService : BaseRepositoryOperations<IReportRepository>, IMarketService
    {
        public MarketService(IReportRepository repository) : base(repository)
        {
        }

        async Task IMarketService.CreateMarketsAsync(MarketRequest[] request)
        {
            await CreateEntitiesAsync(request.Select(createMarketRequest => new Market
            {
                BrandId = createMarketRequest.BrandId,
                Hour = createMarketRequest.Hour,
                ReportDate = createMarketRequest.ReportDate,
                CreatedOn = createMarketRequest.CreatedOn,
                RegisteredCount = createMarketRequest.RegisteredCount,
                FirstFundInCount = createMarketRequest.FirstFundInCount,
                DepositAmount = createMarketRequest.DepositAmount,
                WithdrawalAmount = createMarketRequest.WithdrawalAmount,
                AdjustmentAmount = createMarketRequest.AdjustmentAmount,
                BonusAmount = createMarketRequest.BonusAmount,
                BetAmount = createMarketRequest.BetAmount,
                ValidBetAmount = createMarketRequest.ValidBetAmount,
                WinLossAmount = createMarketRequest.WinLossAmount,
                NetProfit = createMarketRequest.NetProfit
            }).ToArray());
        }

        Task IMarketService.DeleteMarketsAsync(Market[] markets) => DeleteEntitiesAsync(markets);

        Task<bool> IMarketService.ExistsAsync(Expression<Func<Market, bool>> predicate) => ExistsAsync(predicate);

        Task<Market> IMarketService.GetMarketAsync(Expression<Func<Market, bool>> predicate,
            Func<IQueryable<Market>, IQueryable<Market>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Market> IMarketService.GetMarketsAsync(
            Func<IQueryable<Market>, IQueryable<Market>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}