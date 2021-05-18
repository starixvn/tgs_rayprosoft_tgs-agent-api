using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.AgentAdjust;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IAgentTransactionService
    {
        Task CreateAgentTransactionAsync(Agent agent, CreateAgentAdjustmentRequest request);
    }

    public class AgentTransactionService : BaseRepositoryOperations<ITgsRepository>, IAgentTransactionService
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _repository;

        public AgentTransactionService(ITgsRepository repository, IPasswordHandler passwordHandler) : base(repository)
        {
            _passwordHandler = passwordHandler;
            _repository = repository;
        }

        async Task IAgentTransactionService.CreateAgentTransactionAsync(Agent agent, CreateAgentAdjustmentRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var wallet = agent.Wallets.First(x => x.Id == agent.DefaultWalletId);
            var amount = request.AdjustmentType == AdjustmentType.Add
                ? wallet.Balance + request.Amount
                : wallet.Balance - request.Amount;

            wallet.Balance = amount;

            var agentTransaction = new AgentTransaction
            {
                WalletId = agent.DefaultWalletId,
                BrandId = agent.BrandId,
                Type = TransactionType.Adjust,
                Amount = request.Amount,
                Balance = amount,
                RecordedOn = DateTimeOffset.Now
            };

            await CreateEntityAsync(agentTransaction).ConfigureAwait(false);
            await UpdateEntityAsync(wallet).ConfigureAwait(false);

            scope.Complete();
        }
    }
}
