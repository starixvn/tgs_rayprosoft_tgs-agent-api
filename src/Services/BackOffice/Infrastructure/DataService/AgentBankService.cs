using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Agent;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IAgentBankService
    {
        Task<AgentBank> GetAgentBankAsync(Expression<Func<AgentBank, bool>> predicate,
            Func<IQueryable<AgentBank>, IQueryable<AgentBank>> preQuery = null);

        IQueryable<AgentBank> GetAgentBanksAsync(Func<IQueryable<AgentBank>, IQueryable<AgentBank>> preQuery = null,
            bool tracking = false);

        Task CreateAgentBankAsync(CreateAgentBankRequest request);
        Task UpdateAgentBankAsync(AgentBank agentBank, UpdateAgentBankRequest request);
        Task<bool> ExistsAsync(Expression<Func<AgentBank, bool>> predicate);
    }

    public class AgentBankService : BaseRepositoryOperations<ITgsRepository>, IAgentBankService
    {
        private readonly ITgsRepository _repository;
        private readonly ITransactionManager _transactionManager;

        public AgentBankService(ITgsRepository repository, ITransactionManager transactionManager) : base(repository)
        {
            _repository = repository;
            _transactionManager = transactionManager;
        }

        async Task IAgentBankService.CreateAgentBankAsync(CreateAgentBankRequest request)
        {
            var agentBank = new AgentBank
            {
                BankName = request.BankName, RealName = request.RealName, BankAccount = request.BankAccount
            };

            await CreateEntityAsync(agentBank);
        }

        Task<bool> IAgentBankService.ExistsAsync(Expression<Func<AgentBank, bool>> predicate) => ExistsAsync(predicate);

        Task<AgentBank> IAgentBankService.GetAgentBankAsync(Expression<Func<AgentBank, bool>> predicate,
            Func<IQueryable<AgentBank>, IQueryable<AgentBank>> preQuery) => throw new NotImplementedException();

        IQueryable<AgentBank> IAgentBankService.GetAgentBanksAsync(
            Func<IQueryable<AgentBank>, IQueryable<AgentBank>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task IAgentBankService.UpdateAgentBankAsync(AgentBank agentBank, UpdateAgentBankRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            return _transactionManager.ExecuteTransactionAsync(() => _repository,
                IsolationLevel.ReadCommitted, id,
                async _ =>
                {
                    agentBank.RealName = request.RealName;
                    agentBank.BankName = request.BankName;
                    agentBank.BankAccount = request.BankAccount;
                    agentBank.UpdatedByUsername = request.EditorUsername;
                    agentBank.UpdatedOn = DateTimeOffset.Now;

                    var auditLog = new AgentAuditLog
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        AgentId = agentBank.AgentId,
                        CreatedByUsername = request.EditorUsername,
                        CreatedOn = DateTimeOffset.Now,
                        Operation = AgentOperation.Bank
                    };

                    await UpdateEntityAsync(agentBank);
                    await CreateEntityAsync(auditLog);

                    return Task.CompletedTask;
                });
        }
    }
}