using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Services.Report.Application.Models.Player;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IPlayerAggregateService
    {
        ITgsRepository Repository { get; }

        IQueryable<PlayerAggregate> GetPlayerAggregateQueries(
            Func<IQueryable<PlayerAggregate>, IQueryable<PlayerAggregate>> preQuery = null,
            bool tracking = false);

        Task CreatePlayerAggregatesAsync(PlayerAggregateModel[] request);
        Task DeletePlayerAggregatesAsync(PlayerAggregate[] markets);
        Task<bool> ExistsAsync(Expression<Func<PlayerAggregate, bool>> predicate);
    }

    public class PlayerAggregateService : BaseRepositoryOperations<ITgsRepository>, IPlayerAggregateService
    {
        private readonly ITgsRepository _repository;

        public PlayerAggregateService(ITgsRepository repository, ITgsRepository tgsRepository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IPlayerAggregateService.Repository => _repository;

        Task IPlayerAggregateService.CreatePlayerAggregatesAsync(PlayerAggregateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PlayerAggregate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                PlayerCode = model.PlayerCode,
                Hour = model.Hour,
                ReportDate = model.ReportDate,
                CreatedOn = model.CreatedOn,
                RegisteredCount = model.RegisteredCount,
                FirstFundInCount = model.FirstFundInCount,
                DepositAmount = model.DepositAmount.Truncate(2),
                WithdrawalAmount = model.WithdrawalAmount.Truncate(2),
                AdjustmentAmount = model.AdjustmentAmount.Truncate(2),
                BonusAmount = model.BonusAmount.Truncate(2),
                RebateAmount = model.RebateAmount.Truncate(2),
                PromotionAmount = model.PromotionAmount.Truncate(2),
                DividendAmount = model.DividendAmount.Truncate(2),
                BetAmount = model.BetAmount.Truncate(2),
                Turnover = model.Turnover.Truncate(2),
                WinLossAmount = model.WinLossAmount.Truncate(2)
            }).ToArray());

        Task IPlayerAggregateService.DeletePlayerAggregatesAsync(PlayerAggregate[] markets) =>
            DeleteEntitiesAsync(markets);

        Task<bool> IPlayerAggregateService.ExistsAsync(Expression<Func<PlayerAggregate, bool>> predicate) =>
            ExistsAsync(predicate);

        IQueryable<PlayerAggregate> IPlayerAggregateService.GetPlayerAggregateQueries(
            Func<IQueryable<PlayerAggregate>, IQueryable<PlayerAggregate>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}