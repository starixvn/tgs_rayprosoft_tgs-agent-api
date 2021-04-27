using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.Models.Bonus;
using TGS.Services.Bonus;

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

        public async Task UpdatePromotionBonusSentOn(IEnumerable<PromotionPlayerBonus> promotionPlayerBonus)
        {
            var players =
                promotionPlayerBonus.Select(x => x.PromotionBonusId);

            var promotionPlayers = await _repository.PromotionPlayers.Where(x =>
                    players.Contains(x.PromotionBonusId))
                .ToArrayAsync();

            var playerBonus =
                promotionPlayerBonus.SelectMany(x => x.PlayerBonus.Select(p => new {x.PromotionBonusId, p.PlayerCode}))
                    .ToArray();

            var updatePlayers = promotionPlayers.Where(x =>
                    playerBonus.Any(p => p.PlayerCode == x.PlayerCode && p.PromotionBonusId == x.PromotionBonusId))
                .ToArray();

            foreach (var updatePlayer in updatePlayers)
            {
                updatePlayer.SentOn = DateTimeOffset.Now;
            }

            await UpdateEntitiesAsync(updatePlayers);
        }

        async Task IReport.UpdateAgentDividendSentOn(IEnumerable<AgentDividend> agentDividends)
        {
            var agentMinDate =
                agentDividends.Min(x => x.From);
            var agentMaxDate =
                agentDividends.Min(x => x.To);
            var brandIds = agentDividends.Select(x => x.BrandId);
            var agentIds = agentDividends.Select(x => x.AgentId);

            var dividends = await _repository.AgentDividends.Where(x =>
                    brandIds.Contains(x.BrandId))
                .Where(x => agentIds.Contains(x.AgentId))
                .Where(x => x.From >= agentMinDate && x.To <= agentMaxDate)
                .ToArrayAsync();

            var updateAgentDividends = dividends.Where(x => agentDividends.Any(a =>
                a.BrandId == x.BrandId && a.AgentId == x.AgentId && a.From == x.From && a.To == x.To));

            foreach (var updateAgentDividend in updateAgentDividends)
            {
                updateAgentDividend.SentOn = DateTimeOffset.Now;
            }

            await UpdateEntitiesAsync(updateAgentDividends);
        }

        async Task IReport.UpdatePlayerRebateSentOn(IEnumerable<PlayerRebates> playerRebates)
        {
            var playerReportDates =
                playerRebates.Select(x => x.ReportDate);

            var promotionPlayers = await _repository.PlayerRebates
                .Where(x => playerReportDates.Contains(x.ReportDate))
                .ToArrayAsync();

            var players = promotionPlayers.Where(x =>
                playerRebates.Any(p => p.PlayerCode == x.PlayerCode && p.ReportDate == x.ReportDate));

            foreach (var player in players)
            {
                player.SentOn = DateTimeOffset.Now;
            }

            await UpdateEntitiesAsync(players);
        }
    }
}