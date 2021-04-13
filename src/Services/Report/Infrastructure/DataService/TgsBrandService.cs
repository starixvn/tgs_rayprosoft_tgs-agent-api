using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.Agent;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsBrandService
    {
        ITgsRepository Repository { get; }

        IQueryable<Brand> GetBrandQueries(
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null,
            bool tracking = false);

        Task UpdateBrandsAsync(IEnumerable<Brand> brands);
        Task CreateAgentTransactionsAsync(CreateAgentDividendModel[] models);
    }

    public class TgsBrandService : BaseRepositoryOperations<ITgsRepository>, ITgsBrandService
    {
        private readonly ITgsRepository _repository;

        public TgsBrandService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository ITgsBrandService.Repository => _repository;

        async Task ITgsBrandService.CreateAgentTransactionsAsync(CreateAgentDividendModel[] models)
        {
            var agentIds = models.Select(x => x.AgentId).ToArray();

            var agentDividends = models.GroupBy(x => new {x.BrandId, x.AgentId},
                    (x, list) => new {x.AgentId, x.BrandId, list})
                .Join(_repository.Agents.Include(x => x.Wallets).Where(x => agentIds.Contains(x.Id)),
                    m => m.AgentId, a => a.Id,
                    (m, a) => new {m.AgentId, m.BrandId, agentDividends = m.list, agent = a})
                .Select(x =>
                {
                    var wallet = x.agent.Wallets.FirstOrDefault(w => w.Id == x.agent.DefaultWalletId);
                    var dividendAmount = x.agentDividends.Sum(p => p.AgentDividendAmount);
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

            await CreateEntitiesAsync(agentDividends);
        }

        IQueryable<Brand> ITgsBrandService.GetBrandQueries(
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task ITgsBrandService.UpdateBrandsAsync(IEnumerable<Brand> brands) => UpdateEntitiesAsync(brands);
    }
}