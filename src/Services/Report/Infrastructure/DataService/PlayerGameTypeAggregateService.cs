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
    public interface IPlayerGameTypeAggregateService
    {
        ITgsRepository Repository { get; }

        IQueryable<PlayerGameTypeAggregate> GetPlayerGameTypeAggregateQueries(
            Func<IQueryable<PlayerGameTypeAggregate>, IQueryable<PlayerGameTypeAggregate>> preQuery = null,
            bool tracking = false);

        Task CreatePlayerGameTypeAggregatesAsync(PlayerGameTypeAggregateModel[] request);
        Task DeletePlayerGameTypeAggregatesAsync(PlayerGameTypeAggregate[] markets);
        Task<bool> ExistsAsync(Expression<Func<PlayerGameTypeAggregate, bool>> predicate);
    }

    public class PlayerGameTypeAggregateService : BaseRepositoryOperations<ITgsRepository>,
        IPlayerGameTypeAggregateService
    {
        private readonly ITgsRepository _repository;

        public PlayerGameTypeAggregateService(ITgsRepository repository, ITgsRepository tgsRepository) :
            base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IPlayerGameTypeAggregateService.Repository => _repository;

        Task IPlayerGameTypeAggregateService.CreatePlayerGameTypeAggregatesAsync(
            PlayerGameTypeAggregateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PlayerGameTypeAggregate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                GameProviderId = model.GameProviderId,
                PlayerCode = model.PlayerCode,
                VipLevel = model.VipLevel,
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

        Task IPlayerGameTypeAggregateService.DeletePlayerGameTypeAggregatesAsync(PlayerGameTypeAggregate[] markets) =>
            DeleteEntitiesAsync(markets);

        Task<bool> IPlayerGameTypeAggregateService.ExistsAsync(
            Expression<Func<PlayerGameTypeAggregate, bool>> predicate) =>
            ExistsAsync(predicate);

        IQueryable<PlayerGameTypeAggregate> IPlayerGameTypeAggregateService.GetPlayerGameTypeAggregateQueries(
            Func<IQueryable<PlayerGameTypeAggregate>, IQueryable<PlayerGameTypeAggregate>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}