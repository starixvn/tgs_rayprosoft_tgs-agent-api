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
        private readonly IHashProvider _hashProvider;
        private readonly ITokenProvider _tokenProvider;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public BaseGameProviderWalletAction(
            IHashProvider hashProvider,
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations
            )
        {
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        public async ValueTask<GameProviderWalletFnudOutResponse> FnudOutAsync(GameProviderWalletFnudOutRequest request)
        {

            /*
             * 扣錢
             */

            await GameProviderFundAsync();
            /*
             * 建立Game交易單
             */


            return new() { };
        }

        public async ValueTask<GameProviderWalletFundInResponse> FundInAsync(GameProviderWalletFundInRequest request)
        {


            await GameProviderFundAsync();

            /*
             * 加錢
             */
            /*
             * 建立Game交易單
             */

            return new() { };
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

        public abstract ValueTask GameProviderFundAsync();
        public abstract ValueTask<decimal?> GameProviderGetBalanceAsync(CachedPlayer player);
    }
}
