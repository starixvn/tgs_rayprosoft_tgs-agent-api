using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameProviders.Repositories;
using GameProviders.Repositories.Models.GameProviderFundTransferTransaction;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Services.WalletService;

namespace TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction
{
    public abstract record BaseGameProviderWalletAction : IGameProviderWalletAction
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private readonly IWalletRepository _walletRepository;

        public BaseGameProviderWalletAction(
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IWalletRepository walletRepository
            )

        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        /// <summary>
        /// Funds the out asynchronous.
        /// Tgs -> Gp
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderWalletFundResponse> FundOutAsync(GameProviderWalletFundRequest request)
        {
            var fundTransferStatus = FundTransferTransactionStatus.WaitForResponse;
            var amount = Math.Abs(request.Amount);
            var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);
            if (player == null)
            {
                throw new PlayerNotFoundException(request.PlayerCode);
            }
            var playerBalance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            if (playerBalance - amount < 0)
            {
                throw new PlayerHasInsufficientFundsException();
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            /*
            * 建立Game交易單
            */
            var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new GameProviderFundTransferTransactionAddRequest()
            {
                TransactionId = null,
                GameProviderId = gameProviderId,
                WalletId = player.DefaultWalletId,
                GameProviderTransactionId = string.Empty,
                PlayerCode = player.PlayerCode,
                FundSource = FundSource.FundOut,
                FromWallet = "Default",
                ToWallet = request.GameProviderCode,
                Amount = amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });



            #region 扣掉主錢包
            var wallet = await WalletExchangeAsync(player, amount * -1);
            #endregion

            fundTransferStatus = wallet.Status;


            var orderId = default(string);
            if (fundTransferStatus == FundTransferTransactionStatus.Success)
            {
                var response = await FundOutAsync(player, amount);
                fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
                if (!response.Success)
                {
                    var checking = await CheckingOrderIdAsync(player, response.OrderId);
                    if (checking.Checking && !string.IsNullOrEmpty(checking.OrderId))
                    {
                        fundTransferStatus = FundTransferTransactionStatus.Success;
                        orderId = response.OrderId;
                    }
                }
                else
                {
                    fundTransferStatus = FundTransferTransactionStatus.Success;
                    orderId = response.OrderId;
                }
            }


            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new GameProviderFundTransferTransactionUpdateRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = wallet.TransactionId,
                GameProviderTransactionId = orderId,
                Status = fundTransferStatus,
                Balance = wallet.PostBalance ?? 0,
                UpdatedOn = DateTimeOffset.UtcNow,
                Description = (fundTransferStatus == FundTransferTransactionStatus.Success) ? string.Empty : (wallet.Exception == null) ? string.Empty : wallet.Exception.Message
            });

            return new() { GameProviderTransactionId = orderId, TransactionsId = wallet.TransactionId };
        }

        /// <summary>
        /// Funds the in asynchronous.
        /// Gp -> Tgs 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderWalletFundResponse> FundInAsync(GameProviderWalletFundRequest request)
        {
            var amount = Math.Abs(request.Amount);

            var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);
            if (player == null)
            {
                throw new PlayerNotFoundException(request.PlayerCode);
            }
            var responseBalance = await GameProviderGetBalanceAsync(player);
            if (responseBalance == null)
            {
                throw new PlayerNotFoundException(request.PlayerCode);
            }

            if (responseBalance.Value - amount < 0)
            {
                throw new PlayerHasInsufficientFundsException();
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            /*
          * 建立Game交易單
          */
            var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new GameProviderFundTransferTransactionAddRequest()
            {
                TransactionId = null,
                GameProviderId = gameProviderId,
                WalletId = player.DefaultWalletId,
                GameProviderTransactionId = string.Empty,
                PlayerCode = player.PlayerCode,
                FundSource = FundSource.FundIn,
                FromWallet = request.GameProviderCode,
                ToWallet = "Default",
                Amount = request.Amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });

            var response = await FundInAsync(player, Math.Abs(request.Amount));

            FundTransferTransactionStatus fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
            if (!response.Success)
            {
                var checking = await CheckingOrderIdAsync(player, response.OrderId);
                if (checking.Checking && !string.IsNullOrEmpty(checking.OrderId))
                {
                    fundTransferStatus = FundTransferTransactionStatus.Success;
                }
            }
            else
            {
                fundTransferStatus = FundTransferTransactionStatus.Success;
            }


            var transactionsId = default(Guid?);
            var postBalance = default(decimal?);
            var exception = default(Exception);
            if (fundTransferStatus == FundTransferTransactionStatus.Success)
            {

                var wallet = await WalletExchangeAsync(player, amount);
                fundTransferStatus = wallet.Status;
                transactionsId = wallet.TransactionId;
                postBalance = wallet.PostBalance;
                exception = wallet.Exception;

            }

            /*
           * 更新Game交易單
           */
            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new GameProviderFundTransferTransactionUpdateRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = transactionsId ?? null,
                GameProviderTransactionId = response.OrderId,
                Status = fundTransferStatus,
                Balance = postBalance ?? 0,
                UpdatedOn = DateTimeOffset.UtcNow,
                Description = (fundTransferStatus == FundTransferTransactionStatus.Success) ? string.Empty : (exception == null) ? string.Empty : exception.Message
            });


            return new() { GameProviderTransactionId = response.OrderId, TransactionsId = transactionsId };
        }

        /// <summary>
        /// Gets the balance asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(GameProviderWalletGetBalanceRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);

            if (player == null)
            {
                throw new PlayerNotFoundException(request.PlayerCode);
            }

            var balance = await GameProviderGetBalanceAsync(player);

            return new() { Success = balance.HasValue, Balance = balance ?? 0 };
        }

        private async ValueTask<(FundTransferTransactionStatus Status, Guid? TransactionId, decimal? PostBalance, Exception Exception)> WalletExchangeAsync(CachedPlayer player, decimal amount)
        {
            try
            {
                var wallet = await _walletRepository.ExchangeAsync(
                    player.BrandId,
                    player.DefaultWalletId,
                    player.PlayerCode, TransactionTypes.GameProviderFundOut,
                    amount
                    ).ConfigureAwait(false);
                return new(
                    FundTransferTransactionStatus.Success,
                    wallet.TransactionId,
                    wallet.PostBalance,
                    null);
            }
            catch (Exception ex)
            {
                return new(
                 FundTransferTransactionStatus.FailureOnSystem,
                 null,
                 null,
                 ex);
            }
        }

        public abstract ValueTask<(bool Success, string OrderId, decimal Balance)> FundOutAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<(bool Success, string OrderId, decimal Balance)> FundInAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId);

        public abstract ValueTask<decimal?> GameProviderGetBalanceAsync(CachedPlayer player);

    }
}
