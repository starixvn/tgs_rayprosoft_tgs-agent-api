using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bonus;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.Models.Bonus;

namespace Report.Repositories
{
    public class UpdateSentOn : BaseRepositoryOperations<IReportRepository>, IReport
    {
        private readonly IReportRepository _repository;

        public UpdateSentOn(IReportRepository repository) : base(repository)
        {
            _repository = repository;
        }

        IReportRepository IReport.Repository => _repository;

        public async Task UpdatePromotionBonusSentOn(IEnumerable<PromotionPlayerBonus> playerBonus)
        {
            var players =
                playerBonus.SelectMany(x => x.PlayerBonus.Select(p => new {p.PlayerCode, x.PromotionBonusId}));
            var promotionPlayers = await _repository.PromotionPlayers.Where(x =>
                    players.Any(p => p.PlayerCode == x.PlayerCode && p.PromotionBonusId == x.PromotionBonusId))
                .ToArrayAsync();

            foreach (var promotionPlayer in promotionPlayers)
            {
                promotionPlayer.SentOn = DateTimeOffset.Now;
            }

            await UpdateEntitiesAsync(players);
        }

        Task IReport.UpdateAgentDividendSentOn(IEnumerable<AgentDividend> agentDividends) =>
            throw new NotImplementedException();

        async Task IReport.UpdatePlayerRebateSentOn(IEnumerable<PlayerRebates> playerRebates)
        {
            var players =
                playerRebates.Select(x => new {x.PlayerCode, x.ReportDate});
            var promotionPlayers = await _repository.PlayerRebates.Where(x =>
                    players.Any(p => p.PlayerCode == x.PlayerCode && p.ReportDate == x.ReportDate))
                .ToArrayAsync();

            foreach (var promotionPlayer in promotionPlayers)
            {
                promotionPlayer.SentOn = DateTimeOffset.Now;
            }

            await UpdateEntitiesAsync(players);
        }
    }
}