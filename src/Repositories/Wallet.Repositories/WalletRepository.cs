using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Services.Wallet;

namespace TGS.Wallet.Repositories
{
    internal class WalletRepository : IWalletRepository
    {
        private readonly ITgsRepository _tgsRepository;
        private readonly ITransactionManager _transcationManager;

        public WalletRepository(ITgsRepository tgsRepository, ITransactionManager transcationManager)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _transcationManager = transcationManager ?? throw new ArgumentNullException(nameof(transcationManager));
        }

        public async ValueTask<(Guid TransactionId, decimal PostBalance)> ExchangeAsync(
            Guid brandId,
            Guid walletId,
            string playerCode,
            TransactionTypes transactionType,
            decimal amount)
        {
            const string sql = "dbo.wallet_update_wallet_balance";
            var updatedOn = DateTimeOffset.UtcNow;
            var transactionId = GuidExtensions.NewSequentialGuid();

            var postBalance = await _transcationManager
                .ExecuteTransactionAsync(
                    () => _tgsRepository,
                    IsolationLevel.ReadCommitted,
                    transactionId.ToString(),
                    async ctx =>
                    {
                        var postBalance = await _tgsRepository.SqlQuerySingleAsync<decimal>(
                            sql,
                            parameters: new Dictionary<string, object>
                            {
                                {":player_code", playerCode},
                                {":wallet_id", walletId},
                                {":amount", amount},
                                {":updated_on", updatedOn}
                            }).ConfigureAwait(false);

                        await _tgsRepository.Transactions
                            .AddAsync(new Transaction
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
                                RecordedOn = DateTimeOffset.Now
                            })
                            .ConfigureAwait(false);

                        await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

                        return postBalance;
                    })
                .ConfigureAwait(false);

            return (transactionId, postBalance);
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
