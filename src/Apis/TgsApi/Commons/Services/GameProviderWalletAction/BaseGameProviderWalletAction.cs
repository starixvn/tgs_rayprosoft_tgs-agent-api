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

namespace TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction
{
    public abstract record BaseGameProviderWalletAction : IGameProviderWalletAction
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;

        public BaseGameProviderWalletAction(
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository
            )

        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
        }

        /// <summary>
        /// Funds the out asynchronous.
        /// Tgs -> Gp
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderWalletFundResponse> FundOutAsync(GameProviderWalletFundRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new InvalidAmountException(request.Amount.ToString());
            }

            var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);
            if (player == null)
            {
                throw new PlayerNotFoundException(request.PlayerCode);
            }
            var playerBalance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            if (playerBalance - request.Amount < 0)
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
                Amount = request.Amount,
                Balance = 0,
                CreatedOn = DateTimeOffset.UtcNow,
                Description = string.Empty
            });

            /*
             * 扣錢
             */
            var transactionsId = GuidExtensions.NewSequentialGuid();
            playerBalance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

            var fundTransferStatus = FundTransferTransactionStatus.FailureOnSystem;

            /*
             * 扣錢失敗
             */

            var response = await FundOutAsync(player, Math.Abs(request.Amount));
            fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
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

            /*
          * 更新Game交易單
          */
            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new GameProviderFundTransferTransactionUpdateRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = transactionsId,
                GameProviderTransactionId = response.OrderId,
                Status = fundTransferStatus,
                Balance = playerBalance,
                UpdatedOn = DateTimeOffset.UtcNow,
                Description = "完成扣款"
            });

            return new() { GameProviderOrderId = response.OrderId, TransactionsId = transactionsId };
        }

        /// <summary>
        /// Funds the in asynchronous.
        /// Gp -> Tgs 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderWalletFundResponse> FundInAsync(GameProviderWalletFundRequest request)
        {
            if (request.Amount >= 0)
            {
                throw new InvalidAmountException(request.Amount.ToString());
            }

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

            if (responseBalance.Value + request.Amount < 0)
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

            var fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
            if (!response.Success)
            {
                var checking = await CheckingOrderIdAsync(player, response.OrderId);
                if (checking.Checking && !string.IsNullOrEmpty(checking.OrderId))
                {
                    fundTransferStatus = FundTransferTransactionStatus.Success;
                }
                //轉帳失敗
            }
            else
            {
                fundTransferStatus = FundTransferTransactionStatus.Success;
            }

            var transactionsId = default(Guid);
            var playerBalance = default(decimal);
            if (fundTransferStatus == FundTransferTransactionStatus.Success)
            {
                /*
                 * 加錢
                 */
                transactionsId = GuidExtensions.NewSequentialGuid();
                playerBalance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

                /*
                 * 主錢包失敗
                 */
            }

            /*
           * 更新Game交易單
           */
            await _gameProviderFundTransferTransactionRepository.UpdateAsync(new GameProviderFundTransferTransactionUpdateRequest
            {
                GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
                TransactionId = transactionsId,
                GameProviderTransactionId = response.OrderId,
                Status = fundTransferStatus,
                Balance = playerBalance,
                UpdatedOn = DateTimeOffset.UtcNow,
                Description = "完成加錢"
            });


            return new() { GameProviderOrderId = response.OrderId, TransactionsId = transactionsId };
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

        public abstract ValueTask<(bool Success, string OrderId, decimal Balance)> FundOutAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<(bool Success, string OrderId, decimal Balance)> FundInAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId);

        public abstract ValueTask<decimal?> GameProviderGetBalanceAsync(CachedPlayer player);
    }
}
