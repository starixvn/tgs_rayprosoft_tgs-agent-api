using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Report.Application.Models.Agent;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IAgentDividendService
    {
        ITgsRepository Repository { get; }

        IQueryable<AgentDividend> GetAgentDividendQueries(
            Func<IQueryable<AgentDividend>, IQueryable<AgentDividend>> preQuery = null,
            bool tracking = false);

        Task CreateAgentDividendsAsync(CreateAgentDividendModel[] request);
    }

    public class AgentDividendService : BaseRepositoryOperations<ITgsRepository>, IAgentDividendService
    {
        private readonly ITgsRepository _repository;

        public AgentDividendService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IAgentDividendService.Repository => _repository;

        Task IAgentDividendService.CreateAgentDividendsAsync(CreateAgentDividendModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new AgentDividend
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                From = model.From,
                To = model.To,
                AgentDividendPercentage = model.AgentDividendPercentage,
                DepositAmount = model.DepositAmount,
                WithdrawalAmount = model.WithdrawalAmount,
                AdjustmentAmount = model.AdjustmentAmount,
                BonusAmount = model.BonusAmount,
                BetAmount = model.BetAmount,
                Turnover = model.Turnover,
                WinLossAmount = model.WinLossAmount,
                NetProfit = model.NetProfit,
                LiveDealerWinLoss = model.LiveDealerWinLoss,
                SlotWinLoss = model.SlotWinLoss,
                FishingWinLoss = model.FishingWinLoss,
                ArcadeWinLoss = model.ArcadeWinLoss,
                TableGameWinLoss = model.TableGameWinLoss,
                LotteryWinLoss = model.LotteryWinLoss,
                CardGameWinLoss = model.CardGameWinLoss,
                SportsWinLoss = model.SportsWinLoss,
                ESportsWinLoss = model.ESportsWinLoss,
                VirtualSportsWinLoss = model.VirtualSportsWinLoss,
                AgentDividendAmount = model.AgentDividendAmount,
                CreatedOn = model.CreatedOn
            }).ToArray());

        IQueryable<AgentDividend> IAgentDividendService.GetAgentDividendQueries(
            Func<IQueryable<AgentDividend>, IQueryable<AgentDividend>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}