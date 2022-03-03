using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public class SkywindActionModelMapper : ISkywindActionModelMapper
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISkywindService _service;

        public SkywindActionModelMapper(
            ISkywindService service,
            IGameProviderOperations gameProviderOperations)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<BetData> BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source)
        {
            /*
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);

            var cachedGame = cachedGames.First();
            */
            string externalRoundId = _service.GetExternalRoundId(source.RoundId, source.EventId);
            string externalTransactionId = source.TrxId;

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playCode, externalTransactionId).ConfigureAwait(false);
            if (gameActionExternal != null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = externalRoundId,
                ExternalBetId = source.TrxId.ToString(),
                ExternalGameActionGroupId = source.TrxId.ToString(),
                ExternalGameId = source.GameCode.ToString(),
                GameActionType = GameActionType.PlaceBet,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
            };
            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        public async ValueTask<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest source)
        {
            var transactions = default(TransactionData);

            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);
            if (cachedGames.Length == 0)
            {
                return null;
            }
            var cachedGame = cachedGames.First();
            string externalTransactionId = source.TrxId;

            /*
            if (cachedGame.Type == GameType.LiveDealer)
            {
                externalTransactionId = _service.GetExternalTransactionId(source.TrxId, source.EventId, string.Empty);
            }
            */
            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, source.CustId, externalTransactionId).ConfigureAwait(false);
            if (bet == null)
            {
                return null;

            }

            (RoundStatus RoundStatus, int GameId)? round = await _service.GetRoundStatusAndGameIdAsync(data.GameProviderId, playCode, bet.ExternalRoundId, bet.ExternalTransactionId).ConfigureAwait(false);

            if (!round.HasValue || round.Value.RoundStatus == RoundStatus.Closed)
            {
                return null;
            }

            transactions = new TransactionData()
            {
                ExternalGameId = source.GameCode,
                ExternalBetId = bet.ExternalBetId.ToString(),
                ExternalRoundId = bet.ExternalRoundId.ToString(),
                ExternalTransactionId = $"{ bet.ExternalTransactionId}_cancel_bet",
                ExternalTransactionReferenceId = bet.ExternalTransactionId,
                GameActionType = GameActionType.CancelTransaction,
                RoundClosed = true
            };

            return GetBetData(data, playCode, transactions, data.AutoCloseRound);

        }

        public async ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest source)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            var externalRoundId = _service.GetExternalRoundId(source.RoundId, source.EventId);
            var gameActionType = ProcessingSettlement(source.GameType, source.Amount);
            var action = (gameActionType is not GameActionType.FreeBet and not GameActionType.WinJackpot) ? "settle" : gameActionType.ToString().ToLower();
            var autoCloseRound = data.AutoCloseRound;

            var externalTransactionId = $"{source.TrxId}_{action}";
            var externalGameActionGroupId = source.TrxId.ToString();

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playCode, externalTransactionId).ConfigureAwait(false);
            if (gameActionExternal != null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = externalRoundId,
                ExternalBetId = source.TrxId.ToString(),
                ExternalGameActionGroupId = source.TrxId.ToString(),
                ExternalGameId = source.GameCode.ToString(),
                ExternalTransactionReferenceId = externalGameActionGroupId,
                GameActionType = gameActionType,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow
            };

            return GetBetData(data, playCode, transactions, autoCloseRound);
        }

        private static GameActionType ProcessingSettlement(string gameType, decimal amount)
        {
            if (gameType.IgnoreCaseEquals("bonusgame"))
            {
                return GameActionType.WinJackpot;
            }
            else if (gameType.IgnoreCaseEquals("freegame"))
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
