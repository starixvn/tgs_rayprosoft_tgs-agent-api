using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Services.Bonus;

namespace Bonus.Repositories
{
    public class AgentDividendRepository : BaseRepositoryOperations<ITgsRepository>, IAgentDividend
    {
        private readonly ITgsRepository _repository;

        public AgentDividendRepository(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IAgentDividend.Repository => _repository;

        public async Task SentAgentDividend(IEnumerable<AgentDividendBonus> agentDividends,
            TransactionType transactionType)
        {
            var agentIds = agentDividends.Select(x => x.AgentId).ToArray();

            var dividends = agentDividends.GroupBy(x => new {x.BrandId, x.AgentId},
                    (x, list) => new {x.AgentId, x.BrandId, list})
                .Join(_repository.Agents.Include(x => x.Wallets).Where(x => agentIds.Contains(x.Id)),
                    m => m.AgentId, a => a.Id,
                    (m, a) => new {m.AgentId, m.BrandId, agentDividends = m.list, agent = a})
                .Select(x =>
                {
                    var wallet = x.agent.Wallets.FirstOrDefault(w => w.Id == x.agent.DefaultWalletId);
                    var dividendAmount = x.agentDividends.Sum(p => p.BonusAmount);
                    wallet.Balance += dividendAmount;
                    wallet.UpdatedOn = DateTimeOffset.Now;

                    return new AgentTransaction
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        WalletId = x.agent.DefaultWalletId,
                        Wallet = wallet,
                        BrandId = x.BrandId,
                        Amount = dividendAmount,
                        Balance = wallet.Balance,
                        Type = TransactionType.Dividend,
                        RecordedOn = DateTimeOffset.Now
                    };
                }).ToArray();

            await CreateEntitiesAsync(dividends);
        }

        async Task IAgentDividend.UpdateAgentDividendSentOn(IEnumerable<AgentDividendBonus> agentDividends)
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
    }
}