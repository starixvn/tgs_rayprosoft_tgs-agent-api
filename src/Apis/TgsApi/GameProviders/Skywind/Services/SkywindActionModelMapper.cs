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

        public async ValueTask<BetData> BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source)
        {
            string externalRoundId = source.RoundId;
            string externalTransactionId = source.TrxId;

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playCode, externalTransactionId).ConfigureAwait(false);
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
            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        public async ValueTask<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest source)
        {
            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, source.CustId, source.TrxId).ConfigureAwait(false);
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

            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        public async ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest source)
        {
            var gameActionType = ProcessingSettlement(source.GameStatus, source.Amount);
            var action = (gameActionType is not GameActionType.FreeBet and not GameActionType.WinJackpot) ? "settle" : gameActionType.ToString().ToLower();
            var autoCloseRound = (gameActionType is not GameActionType.FreeBet and not GameActionType.WinJackpot);
            var externalTransactionId = $"{source.TrxId}_{action}";
            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, source.CustId, source.TrxId).ConfigureAwait(false);
            if (bet == null)
            {
                return null;
            }

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playCode, externalTransactionId).ConfigureAwait(false);
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
                GameActionType = gameActionType,
                Amount = Math.Abs(source.Amount),
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow
            };

            return GetBetData(data, playCode, transactions, autoCloseRound);
        }

        private static GameActionType ProcessingSettlement(string game_status, decimal amount)
        {

            if (game_status.IgnoreCaseEquals("bonusgame"))
            {
                //Table Game 額外判定
                if (amount == 0)
                {
                    return GameActionType.FreeBet;
                }
                else
                {
                    return GameActionType.WinJackpot;
                }
            }
            else if (game_status.IgnoreCaseEquals("freegame"))
            {
                return GameActionType.FreeBet;
            }
            else
            {
                return amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet;
            }
        }

        private BetData GetBetData(
            GameProviderConfigurationData data,
            string playerCode,
            TransactionData transactions,
            bool autoCloseRound,
            bool allowPlaceBetWithDifferentPlatformToken = false) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.Skywind,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                Transactions = new[] { transactions }
            };
    }
}
