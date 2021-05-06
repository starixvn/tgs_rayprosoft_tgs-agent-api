using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
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

        public BaseGameProviderWalletAction(
            IPlayerOperations playerOperations
            )
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
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

            /*
            * 建立Game交易單
            */

            /*
             * 扣錢
             */

            var response = await FundOutAsync(player, Math.Abs(request.Amount));

            /*
           * 更新Game交易單
           */

            if (!response.success)
            {
                //轉帳失敗
            }

            return new() { GameProviderOrderId = response.orderId, TransactionsId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") };
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
            /*
              * 建立Game交易單
              */
            var response = await FundInAsync(player, Math.Abs(request.Amount));
            /*
             * 加錢
             */

            /*
           * 更新Game交易單
           */

            if (!response.success)
            {
                //轉帳失敗
            }

            return new() { GameProviderOrderId = response.orderId, TransactionsId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") };
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

            return new() { Balance = balance ?? 0 };
        }

        public abstract ValueTask<(bool success, string orderId, decimal balance)> FundOutAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<(bool success, string orderId, decimal balance)> FundInAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<decimal?> GameProviderGetBalanceAsync(CachedPlayer player);
    }
}
