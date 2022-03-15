using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Services
{
    internal class DreamGamegModelActionMapper : IDreamGamegModelActionMapper
    {
        private readonly IDreamGameService _service;

        public DreamGamegModelActionMapper(IDreamGameService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<BetData> BetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameTransferRequest request)
        {
            var autoCloseRound = await _service.GetAutoCloseRoundAsync().ConfigureAwait(false);
            var game = await _service.GetGameAsync().ConfigureAwait(false);

            var transactions = new TransactionData
            {
                Amount = Math.Abs(request.Member.Amount),
                ExternalRoundId = request.TicketId.ToString(),
                ExternalTransactionId = request.Data,
                ExternalBetId = request.Data,
                ExternalGameActionGroupId = request.TicketId.ToString(),
                ExternalGameId = game.ExternalId,
                GameActionType = GameActionType.PlaceBet,
                RoundClosed = false,
                TimeStamp = DateTimeOffset.UtcNow,
            };
            return new()
            {
                PlayerCode = playCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.DreamGame,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = false,
                Transactions = new[] { transactions }
            };
        }

        public async ValueTask<BetData> CancelBetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameInformRequest request)
        {
            var autoCloseRound = await _service.GetAutoCloseRoundAsync().ConfigureAwait(false);

            var betOrder = await _service.GetGameActionExternalIdAsync(gameProviderId, playCode, request.Data).ConfigureAwait(false);

            if (betOrder == null)
            {
                return null;
            }

            var externalTransactionId = $"{ request.Data}_cancel";

            var transactions = new TransactionData
            {
                Amount = Math.Abs(request.Member.Amount),
                ExternalRoundId = betOrder.ExternalRoundId,
                ExternalTransactionId = externalTransactionId,
                ExternalBetId = betOrder.ExternalBetId,
                ExternalGameActionGroupId = betOrder.ExternalGameActionGroupId,
                ExternalGameId = betOrder.ExternalGameId,
                ExternalTransactionReferenceId = betOrder.ExternalTransactionId,
                GameActionType = GameActionType.CancelTransaction,
                RoundClosed = true,
                TimeStamp = DateTimeOffset.UtcNow
            };

            return new()
            {
                PlayerCode = playCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.DreamGame,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = false,
                Transactions = new[] { transactions }
            };
        }

        public async ValueTask<BetData> CancelSettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameInformRequest request)
        {
            var autoCloseRound = await _service.GetAutoCloseRoundAsync().ConfigureAwait(false);

            var settleOrder = await _service.GetGameActionExternalIdAsync(gameProviderId, playCode, request.Data).ConfigureAwait(false);

            if (settleOrder == null)
            {
                return null;
            }

            var externalTransactionId = $"{ request.Data}_cancel";

            var transactions = new TransactionData
            {
                Amount = Math.Abs(request.Member.Amount),
                ExternalRoundId = settleOrder.ExternalRoundId,
                ExternalTransactionId = externalTransactionId,
                ExternalBetId = settleOrder.ExternalBetId,
                ExternalGameActionGroupId = settleOrder.ExternalGameActionGroupId,
                ExternalGameId = settleOrder.ExternalGameId,
                ExternalTransactionReferenceId = settleOrder.ExternalTransactionId,
                GameActionType = GameActionType.CancelTransaction,
                RoundClosed = false,
                TimeStamp = DateTimeOffset.UtcNow
            };

            return new()
            {
                PlayerCode = playCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.DreamGame,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = false,
                Transactions = new[] { transactions }
            };
        }

        public async ValueTask<BetData> ResettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameInformRequest request)
        {
            var autoCloseRound = await _service.GetAutoCloseRoundAsync().ConfigureAwait(false);
            var game = await _service.GetGameAsync().ConfigureAwait(false);

            var externalTransactionId = request.Data + "_resettle";

            var betOrder = await _service.GetGameActionExternalIdAsync(gameProviderId, playCode, $"-{request.Data}").ConfigureAwait(false);

            if (betOrder == null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                Amount = Math.Abs(request.Member.Amount),
                ExternalRoundId = betOrder.ExternalRoundId,
                ExternalTransactionId = externalTransactionId,
                ExternalBetId = betOrder.ExternalBetId,
                ExternalGameActionGroupId = betOrder.ExternalGameActionGroupId,
                ExternalGameId = betOrder.ExternalGameId,
                ExternalTransactionReferenceId = betOrder.ExternalTransactionId,
                GameActionType = request.Member.Amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet,
                RoundClosed = true,
                TimeStamp = DateTimeOffset.UtcNow
            };

            return new()
            {
                PlayerCode = playCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.DreamGame,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = false,
                Transactions = new[] { transactions }
            };
        }

        public async ValueTask<BetData> SettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, string playCode, PlatformType platformType, DreamGameTransferRequest request)
        {
            var autoCloseRound = await _service.GetAutoCloseRoundAsync().ConfigureAwait(false);
            var game = await _service.GetGameAsync().ConfigureAwait(false);

            var externalTransactionId = request.Data;

            var betOrder = await _service.GetGameActionExternalIdAsync(gameProviderId, playCode, $"-{request.Data}").ConfigureAwait(false);

            if (betOrder == null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                Amount = Math.Abs(request.Member.Amount),
                ExternalRoundId = betOrder.ExternalRoundId,
                ExternalTransactionId = externalTransactionId,
                ExternalBetId = betOrder.ExternalBetId,
                ExternalGameActionGroupId = betOrder.ExternalGameActionGroupId,
                ExternalGameId = betOrder.ExternalGameId,
                ExternalTransactionReferenceId = betOrder.ExternalTransactionId,
                GameActionType = request.Member.Amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet,
                RoundClosed = true,
                TimeStamp = DateTimeOffset.UtcNow
            };

            return new()
            {
                PlayerCode = playCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.DreamGame,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = false,
                Transactions = new[] { transactions }
            };
        }
    }
}
