using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

        ValueTask<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playerCode, RollbackRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source);
    }

    public class BetConstructActionModelMapper : IBetConstructActionModelMapper
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBetConstructService _service;

        public BetConstructActionModelMapper(
            IBetConstructService service,
            IGameProviderOperations gameProviderOperations)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
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
                ExternalRoundId = externalRoundId,
                ExternalBetId = source.BetId.ToString(),
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "1",
                GameActionType = GameActionType.PlaceBet,
                ExtraData = DateTimeOffset
                    .FromUnixTimeSeconds(Convert.ToInt64(source.Created))
                    .ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
            };

            return GetBetData(data, playerCode, transactions, data.AutoCloseRound);
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.RollbackBetAsync(
            GameProviderConfigurationData data, string playerCode, RollbackRequest source)
        {
            var transaction = new TransactionData();

            var externalTransactionId = source.TransactionId.ToString();

            var bet = await _service.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, externalTransactionId);

            if (bet != null)
            {
                transaction = new TransactionData()
                {
                    ExternalGameId = bet.ExternalGameId,
                    ExternalBetId = bet.ExternalBetId.ToString(),
                    ExternalRoundId = bet.ExternalRoundId.ToString(),
                    ExternalTransactionId = $"{ bet.ExternalTransactionId}_cancel_bet",
                    ExternalTransactionReferenceId = bet.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    RoundClosed = true
                };
            }
            return transaction == null ? null : GetBetData(data, playerCode, transaction, data.AutoCloseRound);
        }

        async ValueTask<BetData> IBetConstructActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, BetResultedRequest source)
        {
            //var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);
            //var cachedGame = cachedGames.First();

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

            var externalTransactionId = $"{source.TransactionId}_settle";
            var externalGameActionGroupId = source.BetId.ToString();

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
                ExternalBetId = source.BetId.ToString(),
                ExternalGameActionGroupId = externalRoundId,
                ExternalGameId = "1",//Temp Fake Data
                ExternalTransactionReferenceId = source.TransactionId.ToString(),
                GameActionType = gameActionType,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.CalcDate)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
                RoundDetail = JsonSerializer.Serialize(source.BetSelectionState)                
            };

            return GetBetData(data, playCode, transactions, autoCloseRound);
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

        private static GameActionType? ProcessingSettlement(BetResultedRequest source)
        {
            return source.BetState switch
            {
                1 => GameActionType.CancelTransaction,
                2 => GameActionType.TieBet,
                3 => GameActionType.LoseBet,
                4 => GameActionType.WinBet,
                5 => source.Amount > 0 ? GameActionType.WinBet : GameActionType.LoseBet,
                _ => null,
            };
        }
    }
}
