using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Evolution.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public interface IEvolutionActionModelMapper
    {
        BetData Bet(GameProviderConfigurationData data, string playCode, EvolutionDebitRequest source);

        ValueTask<BetData> SettleAsync(GameProviderConfigurationData data, string playCode, EvolutionCreditRequest source);

        Task<BetData> CancelAsync(GameProviderConfigurationData data, string playCode, EvolutionCancelRequest source);
    }

    public class EvolutionActionModelMapper : IEvolutionActionModelMapper
    {
        private readonly string _gameProviderCode = Consts.GameProvider.Evolution;
        private readonly IEvolutionServices _services;
        private readonly IGameProviderOperations _gameProviderOperations;

        public EvolutionActionModelMapper(
            IEvolutionServices services,
             IGameProviderOperations gameProviderOperations
            )
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }


        BetData IEvolutionActionModelMapper.Bet(GameProviderConfigurationData data, string playCode, EvolutionDebitRequest source)
        {

            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Transaction.Amount),
                ExternalRoundId = source.Game.ExternalRoundId,
                ExternalTransactionId = source.Transaction.ExternalTransactionId,
                ExternalBetId = source.Transaction.ExternalGameActionGroupId,
                ExternalGameActionGroupId = source.Transaction.ExternalGameActionGroupId,
                ExternalGameId = source.Game.GameDetail.GameTable.ExternalGameId.ToString(),
                GameActionType = GameActionType.PlaceBet,
                TimeStamp = DateTimeOffset.UtcNow,
            };
            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        async ValueTask<BetData> IEvolutionActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, EvolutionCreditRequest source)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, source.Game.GameDetail.GameTable.ExternalGameId).ConfigureAwait(false);

            var cachedGame = cachedGames.First();
            var orders = await _services.GetGameActionExternalIdsAsync(data.GameProviderId, playCode, source.Game.ExternalRoundId, cachedGame.Id, new GameActionType[] { GameActionType.PlaceBet }).ConfigureAwait(false);
            var order = orders.Where(x => x.ExternalBetId == source.Transaction.ExternalGameActionGroupId).First();
            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Transaction.Amount),
                ExternalRoundId = source.Game.ExternalRoundId,
                ExternalTransactionId = source.Transaction.ExternalTransactionId,
                ExternalBetId = source.Transaction.ExternalGameActionGroupId,
                ExternalGameActionGroupId = source.Transaction.ExternalGameActionGroupId,
                ExternalGameId = source.Game.GameDetail.GameTable.ExternalGameId.ToString(),
                ExternalTransactionReferenceId = order.ExternalTransactionId,
                GameActionType = ProcessingSettlement(source),
                TimeStamp = DateTimeOffset.UtcNow
            };

            return GetBetData(data, playCode, transactions);
        }

        async Task<BetData> IEvolutionActionModelMapper.CancelAsync(GameProviderConfigurationData data, string playCode, EvolutionCancelRequest source)
        {
            var transactions = default(TransactionData);

            var orders = await _services.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(data.GameProviderId, playCode, source.Transaction.ExternalGameActionGroupId).ConfigureAwait(false);
            var order = orders.Where(x => x.ExternalTransactionId == source.Transaction.ExternalTransactionId).First();

            if (order != null)
            {
                string action = (order.GameActionType == GameActionType.PlaceBet) ? "cancel_bet" : "cancel_settle";
                transactions = new TransactionData()
                {
                    Amount = Math.Abs(source.Transaction.Amount),
                    ExternalRoundId = source.Game.ExternalRoundId,
                    ExternalTransactionId = $"{ order.ExternalTransactionId}_{action}",
                    ExternalBetId = source.Transaction.ExternalGameActionGroupId,
                    ExternalGameActionGroupId = source.Transaction.ExternalGameActionGroupId,
                    ExternalGameId = source.Game.GameDetail.GameTable.ExternalGameId.ToString(),
                    ExternalTransactionReferenceId = order.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    RoundClosed = false,
                    TimeStamp = DateTimeOffset.UtcNow
                };
            }
            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        private BetData GetBetData(GameProviderConfigurationData data, string playerCode,
           TransactionData transactions, bool allowPlaceBetWithDifferentPlatformToken = false,
           OppositeBetSetting[] oppositeBetSettings = null) =>
           new()
           {
               PlayerCode = playerCode,
               PlatformType = data.PlatformType,
               GameProviderId = data.GameProviderId,
               GameProviderConfigurationId = data.GameProviderConfigurationId,
               GameProviderCode = _gameProviderCode,
               HasCurrencyFromGameProvider = false,
               AutoCloseRound = data.AutoCloseRound,
               AllowCancelCancelTransaction = false,
               AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
               Transactions = new[] { transactions },
               OppositeBetRules = oppositeBetSettings?.Select(o => new OppositeBetRule
               {
                   ExternalGameIds = o.Games,
                   Patterns = o.Patterns?.Select(p => new OppositeBetRule.Pattern(p.BetType, p.GroupId)).ToArray()
               }).ToArray()
           };

        private GameActionType ProcessingSettlement(EvolutionCreditRequest source)
        {
            if (source.Transaction.Amount > 0)
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
