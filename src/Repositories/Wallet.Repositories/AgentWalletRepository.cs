using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.Wallet;
using TGS.Services.WalletService;

namespace TGS.Wallet.Repositories
{
    internal class AgentWalletRepository : IAgentWalletRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public AgentWalletRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<Guid> ExchangeAsync(Guid brandId, Guid walletId, Guid agentId,
            TransactionTypes transactionType, decimal amount, string? description)
        {
            const string sql = "dbo.agent_wallet_update_wallet_balance";
            var updatedOn = DateTimeOffset.UtcNow;
            var transactionId = GuidExtensions.NewSequentialGuid();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var postBalance = await _tgsRepository.SqlQuerySingleAsync<decimal>(
                sql,
                parameters: new Dictionary<string, object>
                {
                                {":agent_id", agentId},
                                {":wallet_id", walletId},
                                {":amount", amount},
                                {":updated_on", updatedOn}
                }).ConfigureAwait(false);

            await _tgsRepository.AgentTransactions
                .AddAsync(new()
                {
                    Id = transactionId,
                    WalletId = walletId,
                    BrandId = brandId,
                    Amount = amount,
                    Balance = postBalance,
                    Type = transactionType switch
                    {
                        TransactionTypes.Deposit => TransactionType.Deposit,
                        TransactionTypes.Withdrawal => TransactionType.Withdrawal,
                        TransactionTypes.Adjust => TransactionType.Adjust,
                        TransactionTypes.Bonus => TransactionType.Bonus,
                        TransactionTypes.Rebate => TransactionType.Rebate,
                        TransactionTypes.Dividend => TransactionType.Dividend,
                        TransactionTypes.Promotion => TransactionType.Promotion,
                        _ => TransactionType.Other
                    },
                    Description = description,
                    RecordedOn = DateTimeOffset.Now
                })
                .ConfigureAwait(false);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

            scope.Complete();

            return transactionId;
        }

        public async ValueTask<decimal> GetBalanceAsync(Guid walletId, string playerCode)
        {
            var playerWallet = await _tgsRepository.Wallets.AsAsyncEnumerable()
                .Where(wallet => wallet.PlayerCode == playerCode)
                .SingleAsync()
                .ConfigureAwait(false);

            return playerWallet.Balance;
        }
    }
}
