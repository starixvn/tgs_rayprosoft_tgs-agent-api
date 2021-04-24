using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Report.Application.Models.PlayerRebate;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IPlayerRebateService
    {
        IReportRepository Repository { get; }

        IQueryable<PlayerAggregate> GetPlayerAggregateQueries(
            Func<IQueryable<PlayerAggregate>, IQueryable<PlayerAggregate>> preQuery = null,
            bool tracking = false);

        Task CreatePlayerRebatesAsync(PlayerRebateModel[] request);
    }

    public class PlayerRebateService : BaseRepositoryOperations<IReportRepository>, IPlayerRebateService
    {
        private readonly IReportRepository _repository;

        public PlayerRebateService(IReportRepository repository, ITgsRepository tgsRepository) : base(repository)
        {
            _repository = repository;
        }

        IReportRepository IPlayerRebateService.Repository => _repository;

        Task IPlayerRebateService.CreatePlayerRebatesAsync(PlayerRebateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PlayerRebate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                PlayerCode = model.PlayerCode,
                ReportDate = model.ReportDate,
                CreatedOn = DateTimeOffset.Now,
                VipCardGameRebatePercentage = model.VipCardGameRebatePercentage,
                VipLiveDealerRebatePercentage = model.VipLiveDealerRebatePercentage,
                VipLotteryRebatePercentage = model.VipLotteryRebatePercentage,
                VipSlotRebatePercentage = model.VipSlotRebatePercentage,
                VipSportsRebatePercentage = model.VipSportsRebatePercentage,
                VipVirtualSportsRebatePercentage = model.VipVirtualSportsRebatePercentage,
                SportsTurnover = model.SportsTurnover,
                SportsRebateAmount = model.SportsRebateAmount,
                VirtualSportsTurnover = model.VirtualSportsTurnover,
                VirtualSportsRebateAmount = model.VirtualSportsRebateAmount,
                LiveDealerTurnover = model.LiveDealerTurnover,
                LiveDealerRebateAmount = model.LiveDealerRebateAmount,
                CardGameTurnover = model.CardGameTurnover,
                CardGameRebateAmount = model.CardGameRebateAmount,
                SlotTurnover = model.SlotTurnover,
                SlotRebateAmount = model.SlotRebateAmount,
                LotteryTurnover = model.LotteryTurnover,
                LotteryRebateAmount = model.LotteryRebateAmount,
                RebateAmount = model.RebateAmount
            }).ToArray());

        IQueryable<PlayerAggregate> IPlayerRebateService.GetPlayerAggregateQueries(
            Func<IQueryable<PlayerAggregate>, IQueryable<PlayerAggregate>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}