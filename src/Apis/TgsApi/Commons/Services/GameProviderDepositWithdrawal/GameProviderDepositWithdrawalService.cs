using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
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
        ValueTask<GameProviderDepositWithdrawalResponse> GameProviderDepositAsync(GameProviderDepositWithdrawalRequest request);

        /// <summary>
        /// Games the provider withdrawal(GP->TGS).
        /// </summary>
        ValueTask<GameProviderDepositWithdrawalResponse> GameProviderWithdrawalAsync(GameProviderDepositWithdrawalRequest request);

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
        public async ValueTask<GameProviderDepositWithdrawalResponse> GameProviderDepositAsync(GameProviderDepositWithdrawalRequest request)
        {
            var amount = Math.Abs(request.Amount) * -1;
            var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(request.GameProviderId);
            var playerBalance = await _playerOperations.GetPlayerBalanceAsync(request.PlayerWalletId, false);

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
                WalletId = request.PlayerWalletId,
                GameProviderTransactionId = request.GameProviderTransactionId,
                PlayerCode = request.PlayerCode,
                FundSource = FundSource.FundOut,
                FromWallet = "Default",
                ToWallet = gameProvider.Code,
                Amount = amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });


            var wallet = await _walletRepository.ExchangeAsync(
                   request.BrandId,
                   request.PlayerWalletId,
                   request.PlayerCode,
                   TransactionTypes.GameProviderDeposit,
                   amount,
                   gameProvider.Code
                   ).ConfigureAwait(false);


            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new UpdateGameProviderFundTransferTransactionRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = wallet.TransactionId,
                GameProviderTransactionId = request.GameProviderTransactionId,
                Status = FundTransferTransactionStatus.Success,
                Balance = wallet.PostBalance,
                UpdatedOn = DateTimeOffset.UtcNow,
            });

            return new GameProviderDepositWithdrawalResponse()
            {
                TransactionsId = wallet.TransactionId,
                TransactionGameProviderId = transactionGameProviderId.Value,
                Balance = wallet.PostBalance
            };
        }

        /// <summary>
        /// Games the provider withdrawal(GP->TGS).
        /// </summary>
        public async ValueTask<GameProviderDepositWithdrawalResponse> GameProviderWithdrawalAsync(GameProviderDepositWithdrawalRequest request)
        {
            var amount = Math.Abs(request.Amount);
            var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(request.GameProviderId);

            /*
           * 建立Game交易單
           */
            var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new AddGameProviderFundTransferTransactionRequest()
            {
                TransactionId = null,
                GameProviderId = gameProvider.Id,
                WalletId = request.PlayerWalletId,
                GameProviderTransactionId = request.GameProviderTransactionId,
                PlayerCode = request.PlayerCode,
                FundSource = FundSource.FundIn,
                FromWallet = gameProvider.Code,
                ToWallet = "Default",
                Amount = amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });


            var wallet = await _walletRepository.ExchangeAsync(
                   request.BrandId,
                   request.PlayerWalletId,
                   request.PlayerCode,
                   TransactionTypes.GameProviderWithdrawal,
                   amount,
                   gameProvider.Code
                   ).ConfigureAwait(false);


            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new UpdateGameProviderFundTransferTransactionRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = wallet.TransactionId,
                GameProviderTransactionId = request.GameProviderTransactionId,
                Status = FundTransferTransactionStatus.Success,
                Balance = wallet.PostBalance,
                UpdatedOn = DateTimeOffset.UtcNow,
            });

            return new GameProviderDepositWithdrawalResponse()
            {
                TransactionsId = wallet.TransactionId,
                TransactionGameProviderId = transactionGameProviderId.Value,
                Balance = wallet.PostBalance
            };
        }
    }
}
