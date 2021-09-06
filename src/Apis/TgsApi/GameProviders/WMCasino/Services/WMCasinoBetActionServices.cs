using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.WMCasino.Incoming;
using GameBetResult = TGS.Shared.Game.Models.WMCasino.Common.BetResult;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoBetActionServices
    {
        ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode);

        ValueTask<BetResult> LockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, WalletTransferRequest request);

        ValueTask<BetResult> CancelLockAmountAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, CancelWalletTransferRequest request);

        ValueTask ExecuteBetResultsByRoundAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, string playerCode, GameBetResult[] betResults, bool fromSchedule);
    }

    public class WMCasinoBetActionServices : IWMCasinoBetActionServices
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.WMCasino;

        private readonly IGamePlayActions _actions;
        private readonly IWMCasinoActionModelMapper _map;
        private readonly IWMCasinoServices _services;
        private readonly IWMCasinoUtility _utility;

        public WMCasinoBetActionServices(
            IGamePlayActions actions,
            IWMCasinoServices services,
            IWMCasinoUtility utility,
            IWMCasinoActionModelMapper map)
        {
            _actions = actions;
            _services = services;
            _utility = utility;
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

        public async ValueTask ExecuteBetResultsByRoundAsync(Guid gameProviderId, Guid gameProviderConfigurationId,
            string playerCode, GameBetResult[] betResults, bool fromSchedule = false)
        {
            var data = await _services
                .GetGameProviderConfigurationDataWithDesktopPlatformAsync(gameProviderId, gameProviderConfigurationId)
                .ConfigureAwait(false);

            await PlaceAndSettleBetAsync(data, playerCode, betResults, fromSchedule);

            var results = betResults.Where(b => b.Reset.IgnoreCaseEquals("Y")).ToArray();

            if (results.Length > 0)
            {
                await ResettleOrVoidRoundAsync(data, playerCode, results, fromSchedule);
            }
        }

        private async ValueTask PlaceAndSettleBetAsync(GameProviderConfigurationData data,
            string playerCode, GameBetResult[] betResults, bool fromSchedule)
        {
            var betResult = betResults.First();
            var externalGameActionGroupId =
                _utility.GetExternalRoundIdByBetResult(betResult.RoundId, betResult.SubRoundId);

            var externalIds =
                await _services.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(data.GameProviderId,
                    playerCode, externalGameActionGroupId).ConfigureAwait(false);

            var betData = _map.PlaceAndSettleBetRequest(data, playerCode, betResults, externalIds, fromSchedule);

            if (betData != null)
            {
                await _actions.PlaceAndSettleBetAsync(betData).ConfigureAwait(false);
            }
        }

        private async ValueTask ResettleOrVoidRoundAsync(GameProviderConfigurationData data,
            string playerCode, GameBetResult[] betResults, bool fromSchedule)
        {
            var betResult = betResults.First();
            var externalGameActionGroupId =
                _utility.GetExternalRoundIdByBetResult(betResult.RoundId, betResult.SubRoundId);

            var externalIds =
                await _services.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(data.GameProviderId,
                    playerCode, externalGameActionGroupId).ConfigureAwait(false);

            var groupResults = (from r in betResults
                                group r by r.GameResult.IgnoreCaseContains("cancel")
                                into g
                                select g).ToDictionary(g => g.Key, g => g.ToArray());

            if (groupResults.TryGetValue(false, out var resettleResults) && resettleResults.Length > 0)
            {
                await ResettleAsync(data, playerCode, betResults, externalIds);
            }

            if (groupResults.TryGetValue(true, out var voidResults) && voidResults.Length > 0)
            {
                await VoidRoundAsync(data, playerCode, betResults, externalIds);
            }
        }

        private async ValueTask ResettleAsync(GameProviderConfigurationData data, string playerCode,
            GameBetResult[] betResults, GameActionExternalIds[] externalIds)
        {
            var (cancelData, settleData) = _map.ResettleRequest(data, playerCode, betResults, externalIds);

            if (cancelData != null)
            {
                await _actions.CancelTransactionAsync(cancelData).ConfigureAwait(false);
            }

            if (settleData != null)
            {
                await _actions.SettleBetAsync(settleData).ConfigureAwait(false);
            }
        }

        private async ValueTask VoidRoundAsync(GameProviderConfigurationData data, string playerCode,
            GameBetResult[] betResults, GameActionExternalIds[] externalIds)
        {
            var betData = _map.VoidRoundRequest(data, playerCode, betResults, externalIds);

            if (betData != null)
            {
                await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            }
        }
    }
}