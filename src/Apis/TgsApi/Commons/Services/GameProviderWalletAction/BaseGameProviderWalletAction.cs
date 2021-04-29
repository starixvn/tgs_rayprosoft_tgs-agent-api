using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction
{
    public abstract record BaseGameProviderWalletAction : IGameProviderWalletAction
    {
        private readonly ITokenProvider _tokenProvider;

        private readonly IPlayerOperations _playerOperations;

        public BaseGameProviderWalletAction(
            ITokenProvider tokenProvider,
            IPlayerOperations playerOperations
            )
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask<GameProviderWalletFnudResponse> FnudOutAsync(GameProviderWalletFnudRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);

            var player = await _playerOperations.GetCachedPlayerAsync(token.PlayerCode);

            /*
             * 扣錢
             */

            var response = await FnudInAsync(player, Math.Abs(request.Amount));
            /*
             * 建立Game交易單
             */


            return new() { GameProviderOrderId = response.orderId, TransactionsId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") };
        }

        public async ValueTask<GameProviderWalletFnudResponse> FundInAsync(GameProviderWalletFnudRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);

            var player = await _playerOperations.GetCachedPlayerAsync(token.PlayerCode);


            var response = await FnudOutAsync(player, Math.Abs(request.Amount));
            /*
             * 加錢
             */
            /*
             * 建立Game交易單
             */

            return new() { GameProviderOrderId = response.orderId, TransactionsId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") };
        }

        public async ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(GameProviderWalletGetBalanceRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);

            var player = await _playerOperations.GetCachedPlayerAsync(token.PlayerCode);
            var balance = await GameProviderGetBalanceAsync(player);
            return new() { Balance = balance ?? 0 };
        }

        public abstract ValueTask<(string orderId, decimal balance)> FnudInAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<(string orderId, decimal balance)> FnudOutAsync(CachedPlayer player, decimal amount);

        public abstract ValueTask<decimal?> GameProviderGetBalanceAsync(CachedPlayer player);
    }
}
