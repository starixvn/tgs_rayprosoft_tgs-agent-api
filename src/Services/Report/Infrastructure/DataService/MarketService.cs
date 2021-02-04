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

        Task CreateMarketAsync(CreateMarketRequest request);
    }

    public class MarketService : BaseRepositoryOperations<IReportRepository>, IMarketService
    {
        public MarketService(IReportRepository repository) : base(repository)
        {
        }

        async Task IMarketService.CreateMarketAsync(CreateMarketRequest request)
        {
            var market = new Market
            {
                BrandId = request.BrandId,
                CreatedOn = request.CreatedOn,
                RegisteredCount = request.RegisteredCount,
                FirstFundInCount = request.FirstFundInCount,
                DepositAmount = request.DepositAmount,
                WithdrawalAmount = request.WithdrawalAmount,
                AdjustmentAmount = request.AdjustmentAmount,
                BonusAmount = request.BonusAmount,
                BetAmount = request.BetAmount,
                ValidBetAmount = request.ValidBetAmount,
                WinLossAmount = request.WinLossAmount,
                NetProfit = request.NetProfit
            };

            await CreateEntityAsync(market);
        }

        Task<Market> IMarketService.GetMarketAsync(Expression<Func<Market, bool>> predicate,
            Func<IQueryable<Market>, IQueryable<Market>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Market> IMarketService.GetMarketsAsync(
            Func<IQueryable<Market>, IQueryable<Market>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}