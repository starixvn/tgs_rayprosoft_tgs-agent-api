using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindActionModelMapper
    {
        ValueTask<BetData> BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest source);

        Task<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest source);
    }

    public class SkywindActionModelMapper : ISkywindActionModelMapper
    {
        private readonly string _gameProviderCode = Consts.GameProvider.Skywind;

        private readonly ISkywindServices _services;
        private readonly IGameProviderOperations _gameProviderOperations;

        public SkywindActionModelMapper(
            ISkywindServices services,
            IGameProviderOperations gameProviderOperations
            )
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }


        async ValueTask<BetData> ISkywindActionModelMapper.BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);

            var cachedGame = cachedGames.First();

            string externalRoundId = _services.GetExternalRoundId(source.RoundId, source.EventId);
            string externalTransactionId = source.TrxId;
            if (cachedGame.Type == GameType.LiveDealer)
            {
                externalRoundId = source.RoundId;
                externalTransactionId = _services.GetExternalTransactionId(source.TrxId, source.EventId, string.Empty);
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

        async ValueTask<BetData> ISkywindActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest source)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            string externalRoundId = _services.GetExternalRoundId(source.RoundId, source.EventId);
            var autoCloseRound = data.AutoCloseRound;
            var gameActionType = ProcessingSettlement(source, cachedGame.Type);
            var externalTransactionId = $"{source.TrxId}_settle";
            var externalGameActionGroupId = source.TrxId.ToString();
            if (cachedGame.Type == GameType.LiveDealer)
            {
                string action = (gameActionType == GameActionType.FreeBet) ? "freeBet" : "settle";
                externalRoundId = source.RoundId;
                externalGameActionGroupId = _services.GetExternalTransactionId(source.TrxId, source.EventId, string.Empty);
                externalTransactionId = _services.GetExternalTransactionId(source.TrxId, source.EventId, action);
                autoCloseRound = (gameActionType != GameActionType.FreeBet);
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

        async Task<BetData> ISkywindActionModelMapper.RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest source)
        {
            var transactions = default(TransactionData);

            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.GameCode).ConfigureAwait(false);
            var cachedGame = cachedGames.First();
            string externalTransactionId = source.TrxId;

            if (cachedGame.Type == GameType.LiveDealer)
            {
                externalTransactionId = _services.GetExternalTransactionId(source.TrxId, source.EventId, string.Empty);
            }

            var bet = await _services.GetGameActionExternalIdAsync(data.GameProviderId, source.CustId, externalTransactionId).ConfigureAwait(false);
            if (bet != null)
            {
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
            }


            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        private BetData GetBetData(GameProviderConfigurationData data, string playerCode,
           TransactionData transactions, bool autoCloseRound, bool allowPlaceBetWithDifferentPlatformToken = false,
           OppositeBetSetting[] oppositeBetSettings = null) =>
           new()
           {
               PlayerCode = playerCode,
               PlatformType = data.PlatformType,
               GameProviderId = data.GameProviderId,
               GameProviderConfigurationId = data.GameProviderConfigurationId,
               GameProviderCode = _gameProviderCode,
               HasCurrencyFromGameProvider = false,
               AutoCloseRound = autoCloseRound,
               AllowCancelCancelTransaction = false,
               AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
               Transactions = new[] { transactions },
               OppositeBetRules = oppositeBetSettings?.Select(o => new OppositeBetRule
               {
                   ExternalGameIds = o.Games,
                   Patterns = o.Patterns?.Select(p => new OppositeBetRule.Pattern(p.BetType, p.GroupId)).ToArray()
               }).ToArray()
           };

        private GameActionType ProcessingSettlement(SkywindSettleRequest source, GameType gameType)
        {
            if (source.GameType.IgnoreCaseEquals("freegame") && source.GameType.IgnoreCaseEquals("bonusgame"))
            {
                if (source.GameType.IgnoreCaseEquals("freegame"))
                {
                    return GameActionType.FreeBet;
                }
                else
                {
                    return GameActionType.WinJackpot;

                }
            }
            else
            {
                if (source.GameType.IgnoreCaseEquals("normal") && source.GameStatus.IgnoreCaseEquals("freegame") && gameType == GameType.LiveDealer)
                {
                    return GameActionType.FreeBet;
                }
                else
                {
                    if (source.Amount > 0)
                    {
                        return GameActionType.WinBet;
                    }
                    else
                    {
                        return GameActionType.LoseBet;
                    }
                }
            }
        }

    }
}
