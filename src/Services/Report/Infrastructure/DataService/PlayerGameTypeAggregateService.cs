using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Report.Application.Models.Player;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IPlayerGameTypeAggregateService
    {
        IReportRepository Repository { get; }

        IQueryable<PlayerGameTypeAggregate> GetPlayerGameTypeAggregateQueries(
            Func<IQueryable<PlayerGameTypeAggregate>, IQueryable<PlayerGameTypeAggregate>> preQuery = null,
            bool tracking = false);

        Task CreatePlayerGameTypeAggregatesAsync(PlayerGameTypeAggregateModel[] request);
        Task DeletePlayerGameTypeAggregatesAsync(PlayerGameTypeAggregate[] markets);
        Task<bool> ExistsAsync(Expression<Func<PlayerGameTypeAggregate, bool>> predicate);
    }

    public class PlayerGameTypeAggregateService : BaseRepositoryOperations<IReportRepository>,
        IPlayerGameTypeAggregateService
    {
        private readonly IReportRepository _repository;

        public PlayerGameTypeAggregateService(IReportRepository repository, ITgsRepository tgsRepository) :
            base(repository)
        {
            _repository = repository;
        }

        IReportRepository IPlayerGameTypeAggregateService.Repository => _repository;

        Task IPlayerGameTypeAggregateService.CreatePlayerGameTypeAggregatesAsync(
            PlayerGameTypeAggregateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PlayerGameTypeAggregate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                PlayerCode = model.PlayerCode,
                GameType = model.GameType,
                Hour = model.Hour,
                ReportDate = model.ReportDate,
                CreatedOn = model.CreatedOn,
                LiveDealerRebate = model.LiveDealerRebate,
                LiveDealerAmount = model.LiveDealerAmount,
                LiveDealerValidBetAmount = model.LiveDealerAmount,
                SlotRebate = model.SlotRebate,
                SlotAmount = model.SlotAmount,
                SlotValidBetAmount = model.SlotValidBetAmount,
                CardGameRebate = model.CardGameRebate,
                CardGameAmount = model.CardGameAmount,
                CardGameValidBetAmount = model.CardGameValidBetAmount,
                SportsRebate = model.SportsRebate,
                SportsAmount = model.SportsAmount,
                SportsValidBetAmount = model.SportsValidBetAmount,
                VirtualSportsRebate = model.VirtualSportsRebate,
                VirtualSportsAmount = model.VirtualSportsAmount,
                VirtualSportsValidBetAmount = model.VirtualSportsValidBetAmount
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