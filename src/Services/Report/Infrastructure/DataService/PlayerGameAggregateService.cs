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
    public interface IPlayerGameAggregateService
    {
        ITgsRepository Repository { get; }

        IQueryable<PlayerGameAggregate> GetPlayerGameAggregateQueries(
            Func<IQueryable<PlayerGameAggregate>, IQueryable<PlayerGameAggregate>> preQuery = null,
            bool tracking = false);

        Task CreatePlayerGameAggregatesAsync(PlayerGameTypeAggregateModel[] request);
        Task DeletePlayerGameAggregatesAsync(PlayerGameAggregate[] markets);
        Task<bool> ExistsAsync(Expression<Func<PlayerGameAggregate, bool>> predicate);
    }

    public class PlayerGameAggregateService : BaseRepositoryOperations<ITgsRepository>,
        IPlayerGameAggregateService
    {
        private readonly ITgsRepository _repository;

        public PlayerGameAggregateService(ITgsRepository repository, ITgsRepository tgsRepository) :
            base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IPlayerGameAggregateService.Repository => _repository;

        Task IPlayerGameAggregateService.CreatePlayerGameAggregatesAsync(
            PlayerGameTypeAggregateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PlayerGameAggregate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                GameProviderId = model.GameProviderId,
                PlayerCode = model.PlayerCode,
                VipLevel = model.VipLevel,
                GameId = model.GameId,
                GameType = model.GameType,
                Hour = model.Hour,
                ReportDate = model.ReportDate,
                CreatedOn = model.CreatedOn,
                Turnover = model.Turnover.Truncate(2),
                WinLoss = model.WinLoss.Truncate(2),
                VipRebateAmount = model.VipRebateAmount.Truncate(2),
                VipLiveDealerRebatePercentage = model.VipLiveDealerRebatePercentage,
                VipSlotRebatePercentage = model.VipSlotRebatePercentage,
                VipCardGameRebatePercentage = model.VipCardGameRebatePercentage,
                VipSportsRebatePercentage = model.VipSportsRebatePercentage,
                VipVirtualSportsRebatePercentage = model.VipVirtualSportsRebatePercentage,
                VipLotteryRebatePercentage = model.VipLotteryRebatePercentage,
                AgentDividendPercentage = model.AgentDividendPercentage
            }).ToArray());

        Task IPlayerGameAggregateService.DeletePlayerGameAggregatesAsync(PlayerGameAggregate[] markets) =>
            DeleteEntitiesAsync(markets);

        Task<bool> IPlayerGameAggregateService.ExistsAsync(
            Expression<Func<PlayerGameAggregate, bool>> predicate) =>
            ExistsAsync(predicate);

        IQueryable<PlayerGameAggregate> IPlayerGameAggregateService.GetPlayerGameAggregateQueries(
            Func<IQueryable<PlayerGameAggregate>, IQueryable<PlayerGameAggregate>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}