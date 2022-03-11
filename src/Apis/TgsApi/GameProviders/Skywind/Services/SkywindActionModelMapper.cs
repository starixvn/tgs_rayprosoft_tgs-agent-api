using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public class SkywindActionModelMapper : ISkywindActionModelMapper
    {
        private readonly ISkywindService _service;

        public SkywindActionModelMapper(
            ISkywindService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<BetData> BetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, string playerCode, SkywindBetRequest source)
        {
            string externalRoundId = source.RoundId;
            string externalTransactionId = source.TrxId;

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);
            if (gameActionExternal != null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                ExternalRoundId = externalRoundId,
                ExternalGameActionGroupId = externalTransactionId,
                ExternalBetId = externalTransactionId,
                ExternalTransactionId = externalTransactionId,
                ExternalGameId = source.GameCode.ToString(),
                GameActionType = GameActionType.PlaceBet,
                Amount = Math.Abs(source.Amount),
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
            };
            return new()
            {
                PlayerCode = playerCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.Skywind,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = await _service.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false),
                AllowCancelCancelTransaction = false,
                Transactions = new[] { transactions }
            };
        }

        public async ValueTask<BetData> RollbackBetAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, string playerCode, SkywindRollbackBetRequest source)
        {
            var bet = await _service.GetGameActionExternalIdAsync(gameProviderId, source.CustId, source.TrxId).ConfigureAwait(false);
            if (bet == null)
            {
                return null;
            }
            var transactions = new TransactionData()
            {
                ExternalRoundId = bet.ExternalRoundId.ToString(),
                ExternalGameActionGroupId = bet.ExternalGameActionGroupId,
                ExternalBetId = bet.ExternalBetId.ToString(),
                ExternalTransactionReferenceId = bet.ExternalTransactionId,
                ExternalTransactionId = $"{ bet.ExternalTransactionId}_cancel_bet",
                ExternalGameId = source.GameCode,
                GameActionType = GameActionType.CancelTransaction,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
                RoundClosed = true
            };

            return new()
            {
                PlayerCode = playerCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.Skywind,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = await _service.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false),
                AllowCancelCancelTransaction = false,
                Transactions = new[] { transactions }
            };
        }

        public async ValueTask<BetData> SettleAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, string playerCode, SkywindSettleRequest source)
        {
            (bool RoundClosed, GameActionType GameActionType) = ProcessingSettlement(source.GameStatus, source.Amount);
            var action = (GameActionType is not GameActionType.FreeBet and not GameActionType.WinJackpot) ? "settle" : GameActionType.ToString().ToLower();

            var externalTransactionId = $"{source.TrxId}_{action}";
            var bet = await _service.GetGameActionExternalIdAsync(gameProviderId, source.CustId, source.TrxId).ConfigureAwait(false);
            if (bet == null)
            {
                return null;
            }

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);
            if (gameActionExternal != null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                ExternalRoundId = bet.ExternalRoundId,
                ExternalGameActionGroupId = bet.ExternalGameActionGroupId,
                ExternalBetId = bet.ExternalBetId,
                ExternalTransactionReferenceId = bet.ExternalTransactionId,
                ExternalTransactionId = externalTransactionId,
                ExternalGameId = source.GameCode.ToString(),
                GameActionType = GameActionType,
                RoundClosed = RoundClosed,
                Amount = Math.Abs(source.Amount),
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow
            };

            return new()
            {
                PlayerCode = playerCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.Skywind,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = await _service.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false),
                AllowCancelCancelTransaction = false,
                Transactions = new[] { transactions }
            };
        }

        private static (bool RoundClosed, GameActionType GameActionType) ProcessingSettlement(string game_status, decimal amount)
        {
            var roundClosed = game_status.IgnoreCaseEquals("settled");
            var gameActionType = GameActionType.TieBet;

            if (game_status.IgnoreCaseEquals("settled"))
            {
                gameActionType = amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet;
            }
            else
            {
                if (game_status.IgnoreCaseEquals("bonusgame"))
                {
                    gameActionType = amount > 0 ? GameActionType.WinJackpot : GameActionType.FreeBet;
                }
                else
                {
                    gameActionType = GameActionType.FreeBet;
                }
            }
            return (roundClosed, gameActionType);
        }
    }
}
