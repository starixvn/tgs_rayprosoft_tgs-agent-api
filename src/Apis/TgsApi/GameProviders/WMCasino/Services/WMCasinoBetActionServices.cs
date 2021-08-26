using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoBetActionServices
    {
        ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode);

        ValueTask<BetResult> LockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, WalletTransferRequest request);

        ValueTask<BetResult> CancelLockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, CancelWalletTransferRequest request);
    }

    public class WMCasinoBetActionServices : IWMCasinoBetActionServices
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.WMCasino;

        private readonly IGamePlayActions _actions;
        private readonly IWMCasinoActionModelMapper _map;
        private readonly IWMCasinoServices _services;

        public WMCasinoBetActionServices(
            IGamePlayActions actions,
            IWMCasinoServices services,
            IWMCasinoActionModelMapper map)
        {
            _actions = actions;
            _services = services;
            _map = map;
        }

        public ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode)
            => _actions.GetBalanceAsync(_GameProviderCode, playerCode);

        public async ValueTask<BetResult> LockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, WalletTransferRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync(
                    gameProviderId, gameProviderConfigurationId, playerCode, request.ExternalGameId)
                .ConfigureAwait(false);

            var betData = _map.LockAmountRequest(playerCode, data, request);
            return await _actions.LockAmountAsync(betData).ConfigureAwait(false);
        }

        public async ValueTask<BetResult> CancelLockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, CancelWalletTransferRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync(gameProviderId,
                    gameProviderConfigurationId, playerCode, request.ExternalGameId)
                .ConfigureAwait(false);

            var betData = _map.CancelLockAmountRequest(playerCode, data, request);
            return await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
        }
    }
}