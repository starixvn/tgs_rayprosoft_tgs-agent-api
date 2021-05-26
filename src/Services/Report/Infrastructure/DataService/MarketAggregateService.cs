using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IMarketAggregateService
    {
        ITgsRepository Repository { get; }

        IQueryable<MarketAggregate> GetMarketAggregateQueries(
            Func<IQueryable<MarketAggregate>, IQueryable<MarketAggregate>> preQuery = null,
            bool tracking = false);

        Task CreateMarketAggregatesAsync(MarketAggregate[] request);
        Task<bool> ExistsAsync(Expression<Func<MarketAggregate, bool>> predicate);
    }

    public class MarketAggregateService : BaseRepositoryOperations<ITgsRepository>, IMarketAggregateService
    {
        private readonly ITgsRepository _repository;

        public MarketAggregateService(ITgsRepository repository, ITgsRepository tgsRepository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IMarketAggregateService.Repository => _repository;

        Task IMarketAggregateService.CreateMarketAggregatesAsync(MarketAggregate[] models) =>
            CreateEntitiesAsync(models);

        Task<bool> IMarketAggregateService.ExistsAsync(Expression<Func<MarketAggregate, bool>> predicate) =>
            ExistsAsync(predicate);

        IQueryable<MarketAggregate> IMarketAggregateService.GetMarketAggregateQueries(
            Func<IQueryable<MarketAggregate>, IQueryable<MarketAggregate>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}