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
    public interface IPlayerAggregateService
    {
        IReportRepository Repository { get; }

        IQueryable<PlayerAggregate> GetPlayerAggregateQueries(
            Func<IQueryable<PlayerAggregate>, IQueryable<PlayerAggregate>> preQuery = null,
            bool tracking = false);

        Task CreatePlayerAggregatesAsync(PlayerAggregateModel[] request);
        Task CreatePlayerAggregatesByGameTypeAsync(PlayerGameTypeAggregateModel[] request);
        Task DeletePlayerAggregatesAsync(PlayerAggregate[] markets);
        Task<bool> ExistsAsync(Expression<Func<PlayerAggregate, bool>> predicate);
    }

    public class PlayerAggregateService : BaseRepositoryOperations<IReportRepository>, IPlayerAggregateService
    {
        private readonly IReportRepository _repository;

        public PlayerAggregateService(IReportRepository repository, ITgsRepository tgsRepository) : base(repository)
        {
            _repository = repository;
        }

        IReportRepository IPlayerAggregateService.Repository => _repository;

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
                DepositAmount = model.DepositAmount,
                WithdrawalAmount = model.WithdrawalAmount,
                AdjustmentAmount = model.AdjustmentAmount,
                BonusAmount = model.BonusAmount,
                BetAmount = model.BetAmount,
                ValidBetAmount = model.ValidBetAmount,
                WinLossAmount = model.WinLossAmount,
                NetProfit = model.NetProfit
            }).ToArray());

        Task IPlayerAggregateService.CreatePlayerAggregatesByGameTypeAsync(PlayerGameTypeAggregateModel[] models) =>
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
                SlotRebate = model.SlotRebate,
                SlotAmount = model.SlotAmount,
                CardGameRebate = model.CardGameRebate,
                CardGameAmount = model.CardGameAmount,
                SportsRebate = model.SportsRebate,
                SportsAmount = model.SportsAmount,
                VirtualSportsRebate = model.VirtualSportsRebate,
                VirtualSportsAmount = model.VirtualSportsAmount
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