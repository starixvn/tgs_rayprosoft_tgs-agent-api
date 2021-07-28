using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction;
using TGS.Services.Operations;
using TGS.Services.WalletService;

namespace TGS.Apis.TgsApi.Commons.Services.GameProviderDepositWithdrawal
{
    public interface IGameProviderDepositWithdrawalService
    {
        /// <summary>
        /// Games the provider deposit(TGS->GP).
        /// </summary>
        ValueTask<(Guid transactionsId, Guid transactionGameProviderId, decimal balance)> GameProviderDepositAsync(Guid gameProviderId, CachedPlayer player, string gameProviderTransactionId, decimal amount);

        /// <summary>
        /// Games the provider withdrawal(GP->TGS).
        /// </summary>
        ValueTask<(Guid transactionsId, Guid transactionGameProviderId, decimal balance)> GameProviderWithdrawalAsync(Guid gameProviderId, CachedPlayer player, string gameProviderTransactionId, decimal amount);

    }
    public sealed class GameProviderDepositWithdrawalService : IGameProviderDepositWithdrawalService
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IWalletRepository _walletRepository;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;

        public GameProviderDepositWithdrawalService(
            IPlayerOperations playerOperations,
            IWalletRepository walletRepository,
            IGameProviderOperations gameProviderOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
        }

        /// <summary>
        /// Games the provider deposit(TGS->GP).
        /// </summary>
        public async ValueTask<(Guid transactionsId, Guid transactionGameProviderId, decimal balance)> GameProviderDepositAsync(Guid gameProviderId, CachedPlayer player, string gameProviderTransactionId, decimal amount)
        {
            amount = Math.Abs(amount) * -1;
            var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var playerBalance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            if (playerBalance + amount < 0)
            {
                throw new PlayerHasInsufficientFundsException();
            }

            /*
           * 建立Game交易單
           */
            var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new AddGameProviderFundTransferTransactionRequest()
            {
                TransactionId = null,
                GameProviderId = gameProvider.Id,
                WalletId = player.DefaultWalletId,
                GameProviderTransactionId = gameProviderTransactionId,
                PlayerCode = player.PlayerCode,
                FundSource = FundSource.FundOut,
                FromWallet = "Default",
                ToWallet = gameProvider.Code,
                Amount = amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });


            var wallet = await _walletRepository.ExchangeAsync(
                   player.BrandId,
                   player.DefaultWalletId,
                   player.PlayerCode,
                   TransactionTypes.GameProviderDeposit,
                   amount,
                   gameProvider.Code
                   ).ConfigureAwait(false);


            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new UpdateGameProviderFundTransferTransactionRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = wallet.TransactionId,
                GameProviderTransactionId = gameProviderTransactionId,
                Status = FundTransferTransactionStatus.Success,
                Balance = wallet.PostBalance,
                UpdatedOn = DateTimeOffset.UtcNow,
            });

            return new(wallet.TransactionId, transactionGameProviderId.Value, wallet.PostBalance);
        }

        /// <summary>
        /// Games the provider withdrawal(GP->TGS).
        /// </summary>
        public async ValueTask<(Guid transactionsId, Guid transactionGameProviderId, decimal balance)> GameProviderWithdrawalAsync(Guid gameProviderId, CachedPlayer player, string gameProviderTransactionId, decimal amount)
        {
            amount = Math.Abs(amount);
            var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);

            /*
           * 建立Game交易單
           */
            var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new AddGameProviderFundTransferTransactionRequest()
            {
                TransactionId = null,
                GameProviderId = gameProvider.Id,
                WalletId = player.DefaultWalletId,
                GameProviderTransactionId = gameProviderTransactionId,
                PlayerCode = player.PlayerCode,
                FundSource = FundSource.FundIn,
                FromWallet = gameProvider.Code,
                ToWallet = "Default",
                Amount = amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });


            var wallet = await _walletRepository.ExchangeAsync(
                   player.BrandId,
                   player.DefaultWalletId,
                   player.PlayerCode,
                   TransactionTypes.GameProviderWithdrawal,
                   amount,
                   gameProvider.Code
                   ).ConfigureAwait(false);


            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new UpdateGameProviderFundTransferTransactionRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = wallet.TransactionId,
                GameProviderTransactionId = gameProviderTransactionId,
                Status = FundTransferTransactionStatus.Success,
                Balance = wallet.PostBalance,
                UpdatedOn = DateTimeOffset.UtcNow,
            });

            return new(wallet.TransactionId, transactionGameProviderId.Value, wallet.PostBalance);
        }
    }
}
