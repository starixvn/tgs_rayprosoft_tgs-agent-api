using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
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
        private readonly SystemTransactionManager _transactionManager;

        public AgentTransactionService(ITgsRepository repository, IPasswordHandler passwordHandler,
            SystemTransactionManager transactionManager) : base(repository)
        {
            _passwordHandler = passwordHandler;
            _repository = repository;
            _transactionManager = transactionManager;
        }

        Task IAgentTransactionService.CreateAgentTransactionAsync(Agent agent, CreateAgentAdjustmentRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            return _transactionManager.ExecuteTransactionAsync(_repository,
                IsolationLevel.ReadCommitted, id,
                async _ =>
                {
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

                    await CreateEntityAsync(agentTransaction);
                    await UpdateEntityAsync(wallet);

                    return Task.CompletedTask;
                });
        }
    }
}