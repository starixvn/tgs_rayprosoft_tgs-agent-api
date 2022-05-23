using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson;
using Tgs.MessageQueue;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.BetConstruct.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Services
{
    public interface IBetConstructActionModelMapper
    {
        ValueTask<BetData> BetAsync(GameProviderConfigurationData data, string playerCode, BetPlacedRequest source);

        ValueTask<BetData> CancelSettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source);

        ValueTask<BetData> ReSettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source);

        ValueTask<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playerCode, RollbackRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source);
    }

    public class BetConstructActionModelMapper : IBetConstructActionModelMapper
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IMessageSender _messageSender;
        private readonly IBetConstructService _service;

        public BetConstructActionModelMapper(
            IBetConstructService service,
            IGameProviderOperations gameProviderOperations,
            IMessageSender messageSender)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.BetAsync(
            GameProviderConfigurationData data,
            string playerCode,
            BetPlacedRequest source)
        {
            var externalRoundId = source.BetId.ToString();
            var externalTransactionId = source.TransactionId.ToString();

            var gameActionExternal = await _service
                .GetGameActionExternalIdAsync(data.GameProviderId, playerCode, externalTransactionId)
                .ConfigureAwait(false);

            if (gameActionExternal != null)
            {
                return null;
            }

            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = source.BetId.ToString(),
                ExternalBetId = source.BetId.ToString(),
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "sportsbook",
                GameActionType = GameActionType.PlaceBet,
                TimeStamp = DateTimeOffset.UtcNow,
                RoundDetail = JsonSerializer.Serialize(new { Bet = source.Selections })
            };

            return GetBetData(data, playerCode, transactions, data.AutoCloseRound);
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.CancelSettleAsync(GameProviderConfigurationData data, string playerCode, BetResultedRequest source)
        {
            var transaction = default(TransactionData);
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, "sportsbook").ConfigureAwait(false);
            var cachedGame = cachedGames.First();
            var externalRoundId = source.BetId.ToString();
            var externalTransactionId = source.TransactionId.ToString();

            var (roundRecord, gameActionRecord) = await _service
                .GetSettleGameActionByBetAsync(data.GameProviderId, playerCode, source.BetId.ToString(), cachedGame.Id)
                .ConfigureAwait(false);

            if (gameActionRecord != null)
            {
                transaction = new TransactionData
                {
                    Amount = Math.Abs(source.Amount),
                    ExternalTransactionId = $"{externalTransactionId}_settle_cancel",
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = "sportsbook",
                    ExternalTransactionReferenceId = gameActionRecord.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    ExtraData = DateTimeOffset.FromUnixTimeSeconds(source.CalcDate.ToUnixTimeSeconds()).ToIso8601String(true),
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = false
                };
                await _messageSender
                    .PublishAsync("tgs.gp.gamerecord.database.detail.bc", new { Settle = source.BetSelectionState }.ToBson())
                    .ConfigureAwait(false);
            }

            return GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.ReSettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, "sportsbook").ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            string externalRoundId = source.BetId.ToString();

            var autoCloseRound = data.AutoCloseRound;
            var gameActionType = source.BetState switch
            {
                1 => GameActionType.CancelTransaction,
                2 => GameActionType.TieBet,
                3 => GameActionType.LoseBet,
                4 => GameActionType.WinBet,
                5 => source.Amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet,
                _ => GameActionType.PlaceBet
            };

            var externalTransactionId = $"{source.TransactionId}";

            var (roundRecord, gameActionRecord) = await _service
                .GetGameActionByBetAsync(data.GameProviderId, playCode, source.BetId.ToString(), cachedGame.Id)
                .ConfigureAwait(false);

            var transaction = new TransactionData
            {
                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = externalRoundId,
                ExternalBetId = externalRoundId,
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "sportsbook",
                ExternalTransactionReferenceId = gameActionRecord.ExternalTransactionId,
                GameActionType = gameActionType,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(source.CalcDate.ToUnixTimeSeconds()).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
                RoundClosed = true
            };
            await _messageSender
                    .PublishAsync("tgs.gp.gamerecord.database.detail.bc", new { Settle = source.BetSelectionState }.ToBson())
                    .ConfigureAwait(false);

            return GetBetData(data, playCode, transaction, autoCloseRound);
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.RollbackBetAsync(
            GameProviderConfigurationData data, string playerCode, RollbackRequest source)
        {
            var transaction = default(TransactionData);

            var externalTransactionId = source.TransactionId.ToString();

            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, externalTransactionId);

            if (bet != null)
            {
                transaction = new TransactionData()
                {
                    ExternalGameId = bet.ExternalGameId,
                    ExternalBetId = bet.ExternalBetId.ToString(),
                    ExternalRoundId = bet.ExternalRoundId.ToString(),
                    ExternalTransactionId = $"{bet.ExternalTransactionId}",
                    ExternalTransactionReferenceId = bet.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    RoundClosed = true
                };
            }
            return transaction == null ? null : GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, "sportsbook").ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            var externalRoundId = source.BetId.ToString();

            var autoCloseRound = data.AutoCloseRound;
            var gameActionType = source.BetState switch
            {
                1 => GameActionType.CancelTransaction,
                2 => GameActionType.TieBet,
                3 => GameActionType.LoseBet,
                4 => GameActionType.WinBet,
                5 => source.Amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet,
                _ => GameActionType.PlaceBet
            };

            var externalTransactionId = $"{source.TransactionId}";
            var externalGameActionGroupId = source.BetId.ToString();

            var gameActionExternal = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playCode, externalTransactionId).ConfigureAwait(false);
            if (gameActionExternal != null)
            {
                return null;
            }

            var (roundRecord, gameActionRecord) = await _service
                .GetGameActionByBetAsync(data.GameProviderId, playCode, source.BetId.ToString(), cachedGame.Id)
                .ConfigureAwait(false);

            var transaction = new TransactionData
            {
                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = source.BetId.ToString(),
                ExternalBetId = source.BetId.ToString(),
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "sportsbook",
                ExternalTransactionReferenceId = gameActionRecord.ExternalTransactionId,
                GameActionType = gameActionType,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(source.CalcDate.ToUnixTimeSeconds()).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow
            };

            await _messageSender
                    .PublishAsync("tgs.gp.gamerecord.database.detail.bc", new { Settle = source.BetSelectionState }.ToBson())
                    .ConfigureAwait(false);

            return GetBetData(data, playCode, transaction, autoCloseRound);
        }

        private BetData GetBetData(
            GameProviderConfigurationData data,
            string playerCode,
            TransactionData transactions,
            bool autoCloseRound,
            bool allowPlaceBetWithDifferentPlatformToken = false)
            => new BetData()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.BetConstruct,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                Transactions = new[] { transactions }
            };
    }
}
