using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Card365.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365BetActionServices
    {
        ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode);

        ValueTask<BetResult> LockAmountAsync(string playerCode, BetRequest request);
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

        public ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode)
            => _actions.GetBalanceAsync(_GameProviderCode, playerCode);

        public async ValueTask<BetResult> LockAmountAsync(string playerCode, BetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.LockAmountRequest(playerCode, data, request);
            return await _actions.LockAmountAsync(betData).ConfigureAwait(false);
        }
    }
}