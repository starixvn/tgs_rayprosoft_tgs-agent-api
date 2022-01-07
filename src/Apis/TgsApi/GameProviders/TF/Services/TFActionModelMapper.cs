using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TF.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public interface ITFActionModelMapper
    {
        ValueTask<BetData> BetAsync(GameProviderConfigurationData data, string playerCode, BetPlacedRequest source);

        ValueTask<BetData> CancelRoundBetAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source);

        ValueTask<BetData> CancelRoundSettleAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source);

        ValueTask<BetData> CancelRoundAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source);


        ValueTask<BetData> CancelSettleAsync(GameProviderConfigurationData data, string playerCode, UnSettleRequest source);

        ValueTask<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playerCode, RollbackRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source);
    }

    public class TFActionModelMapper : ITFActionModelMapper
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITFService _service;

        public TFActionModelMapper(
            ITFService service,
            IGameProviderOperations gameProviderOperations)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        async ValueTask<BetData> ITFActionModelMapper.BetAsync(
            GameProviderConfigurationData data,
            string playerCode,
            BetPlacedRequest source)
        {
            var externalRoundId = source.TicketNum;
            var externalTransactionId = source.TicketDetail.Id;

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
                ExternalRoundId = externalRoundId,
                ExternalBetId = externalRoundId,
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "4",
                GameActionType = GameActionType.PlaceBet,
                ExtraData = DateTimeOffset
                    .FromUnixTimeSeconds(source.TicketDetail.DateCreated.ToUnixTimeSeconds())
                    .ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
            };

            return GetBetData(data, playerCode, transactions, data.AutoCloseRound);
        }

        async ValueTask<BetData> ITFActionModelMapper.CancelRoundBetAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source)
        {
            var transaction = default(TransactionData);

            var externalRoundId = source.TicketNum.Split("_")[0];

            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, externalRoundId);

            if (bet != null)
            {
                transaction = new TransactionData()
                {
                    ExternalGameId = bet.ExternalGameId,
                    ExternalBetId = bet.ExternalBetId,
                    ExternalRoundId = bet.ExternalRoundId,
                    ExternalTransactionId = $"{source.TicketNum}_bet_cancel",
                    ExternalTransactionReferenceId = bet.ExternalTransactionId.ToString(),
                    GameActionType = GameActionType.CancelTransaction,
                    RoundClosed = true
                };
            }
            return transaction == null ? null : GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> ITFActionModelMapper.CancelRoundSettleAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source)
        {
            var transaction = default(TransactionData);
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, "4").ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            var externalRoundId = source.TicketNum.Split('_')[0];
            var externalTransactionId = source.TicketNum;

            var (roundRecord, gameActionRecord) = await _service
                .GetSettleGameActionByBetAsync(data.GameProviderId, playerCode, externalRoundId, cachedGame.Id)
                .ConfigureAwait(false);

            if (gameActionRecord != null && gameActionRecord.Type == GameActionType.LoseBet)
            {
                transaction = new TransactionData
                {
                    Amount = Math.Abs(source.Amount),
                    ExternalTransactionId = $"{externalTransactionId}_settle_cancel",
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = "4",
                    ExternalTransactionReferenceId = gameActionRecord.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    ExtraData = DateTimeOffset.FromUnixTimeSeconds(source.TicketDetail.ModifiedDatetime.ToUnixTimeSeconds()).ToIso8601String(true),
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = false,
                    RoundDetail = JsonSerializer.Serialize(source.TicketDetail)
                };
            }
            else
            {
                return null;
            }

            return GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> ITFActionModelMapper.CancelSettleAsync(GameProviderConfigurationData data, string playerCode, UnSettleRequest source)
        {
            var transaction = default(TransactionData);
            var cachedGames = await _gameProviderOperations
                .GetCachedGamesAsync(data.GameProviderId, "4")
                .ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            var externalRoundId = source.TicketNum.Split("_")[0];
            var externalTransactionId = source.TicketDetail.Id;

            var (roundRecord, gameActionRecord) = await _service
                .GetSettleGameActionByBetAsync(data.GameProviderId, playerCode, externalRoundId, cachedGame.Id)
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
                    ExternalGameId = "4",
                    ExternalTransactionReferenceId = gameActionRecord.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    ExtraData = DateTimeOffset.FromUnixTimeSeconds(source.TicketDetail.ModifiedDatetime.ToUnixTimeSeconds()).ToIso8601String(true),
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = false,
                    RoundDetail = JsonSerializer.Serialize(source.TicketDetail)
                };
            }

            return GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> ITFActionModelMapper.RollbackBetAsync(GameProviderConfigurationData data, string playerCode, RollbackRequest source)
        {
            var transaction = default(TransactionData);

            var externalRoundId = source.TicketNum;

            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, externalRoundId);

            if (bet != null)
            {
                transaction = new TransactionData()
                {
                    ExternalGameId = bet.ExternalGameId,
                    ExternalBetId = bet.ExternalBetId,
                    ExternalRoundId = bet.ExternalRoundId,
                    ExternalTransactionId = $"{bet.ExternalTransactionId}_bet_cancel",
                    ExternalTransactionReferenceId = bet.ExternalTransactionId.ToString(),
                    GameActionType = GameActionType.CancelTransaction,
                    RoundClosed = true
                };
            }
            return transaction == null ? null : GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> ITFActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source)
        {
            var externalRoundId = source.TicketNum.Split("_")[0];

            var autoCloseRound = data.AutoCloseRound;

            var gameActionType = source.TicketDetail.ResultStatus switch
            {
                "CANCELLED" => GameActionType.CancelTransaction,
                "DRAW" => GameActionType.TieBet,
                "LOSS" => GameActionType.LoseBet,
                "WIN" => GameActionType.WinBet,
                _ => GameActionType.PlaceBet
            };

            var externalTransactionId = source.TicketDetail.Id;

            var gameActionExternal = await _service
                .GetGameActionExternalIdAsync(data.GameProviderId, playerCode, externalTransactionId)
                .ConfigureAwait(false);

            if (gameActionExternal == null)
                return null;

            var transaction = new TransactionData
            {
                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = source.TicketNum,
                ExternalRoundId = externalRoundId,
                ExternalBetId = externalRoundId,
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "4",
                ExternalTransactionReferenceId = gameActionExternal.ExternalTransactionId,
                GameActionType = gameActionType,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(source.TicketDetail.SettlementDatetime.Value.ToUnixTimeSeconds()).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
                RoundDetail = JsonSerializer.Serialize(source.TicketDetail)
            };

            return GetBetData(data, playerCode, transaction, autoCloseRound);
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
                GameProviderCode = Consts.GameProvider.TFGaming,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                Transactions = new[] { transactions }
            };
        ValueTask<BetData> ITFActionModelMapper.CancelRoundAsync(GameProviderConfigurationData data, string playerCode, SettleRequest source) => throw new NotImplementedException();
    }
}
