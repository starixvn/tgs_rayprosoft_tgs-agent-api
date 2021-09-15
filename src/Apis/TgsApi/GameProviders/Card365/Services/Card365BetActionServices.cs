using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Card365.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365BetActionServices
    {
        ValueTask<BalanceResponse> GetBalanceAsync(string playerCode);

        ValueTask<BalanceResponse> LockAmountAsync(string playerCode, BetRequest request);

        ValueTask<BalanceResponse> CancelLockAmountAsync(string playerCode, CancelBetRequest request);
    }

    public class Card365BetActionServices : ICard365BetActionServices
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.Card365;

        private readonly IGamePlayActions _actions;
        private readonly ICard365ActionModelMapper _map;
        private readonly ICard365Services _services;

        public Card365BetActionServices(
            ICard365Services services,
            IGamePlayActions actions,
            ICard365ActionModelMapper map)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public async ValueTask<BalanceResponse> CancelLockAmountAsync(string playerCode, CancelBetRequest request)
        {
            var externalRoundId = request.ExternalRoundId;

            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var externalIds = await _services
                .GetGameActionExternalIdsAsync(data.GameProviderId, playerCode, externalRoundId).ConfigureAwait(false);

            var lockTransaction = externalIds.FirstOrDefault(a => a.GameActionType == GameActionType.LockAmount);

            if (externalIds.Any(a => a.GameActionType == GameActionType.CancelTransaction &&
                a.ExternalReferencedTransactionId == lockTransaction?.ExternalTransactionId))
            {
                return await GetBalanceAsync(playerCode).ConfigureAwait(false);
            }

            var betData = _map.CancelLockAmountRequest(playerCode, data, externalIds, request);
            var result = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(result);
        }

        public async ValueTask<BalanceResponse> GetBalanceAsync(string playerCode)
        {
            var result = await _actions.GetBalanceAsync(_GameProviderCode, playerCode)
                .ConfigureAwait(false);
            return _map.Response(result);
        }

        public async ValueTask<BalanceResponse> LockAmountAsync(string playerCode, BetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.LockAmountRequest(playerCode, data, request);
            var result = await _actions.LockAmountAsync(betData).ConfigureAwait(false);
            return _map.Response(result);
        }
    }
}