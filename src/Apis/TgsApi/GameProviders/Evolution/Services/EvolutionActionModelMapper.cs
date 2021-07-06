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

        BetData Settle(GameProviderConfigurationData data, string playCode, EvolutionCreditRequest source);

        Task<BetData> CancelAsync(GameProviderConfigurationData data, string playCode, EvolutionCancelRequest source);
    }

    public class EvolutionActionModelMapper : IEvolutionActionModelMapper
    {
        private readonly string _gameProviderCode = Consts.GameProvider.Evolution;
        private readonly IEvolutionServices _services;


        public EvolutionActionModelMapper(
            IEvolutionServices services
            )
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }


        BetData IEvolutionActionModelMapper.Bet(GameProviderConfigurationData data, string playCode, EvolutionDebitRequest source)
        {

            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Transaction.Amount),
                ExternalTransactionId = source.Transaction.Id,
                ExternalRoundId = source.Game.Id,
                ExternalBetId = source.Transaction.RefId,
                ExternalGameActionGroupId = source.Transaction.RefId,
                ExternalGameId = source.Game.GameDetail.GameTable.Id.ToString(),
                GameActionType = GameActionType.PlaceBet,
                TimeStamp = DateTimeOffset.UtcNow,
            };
            return GetBetData(data, playCode, transactions, data.AutoCloseRound);
        }

        BetData IEvolutionActionModelMapper.Settle(GameProviderConfigurationData data, string playCode, EvolutionCreditRequest source)
        {
            var transactions = new TransactionData
            {
                Amount = Math.Abs(source.Transaction.Amount),
                ExternalTransactionId = $"{ source.Transaction.Id}_settle",
                ExternalRoundId = source.Game.Id,
                ExternalBetId = source.Transaction.RefId,
                ExternalGameActionGroupId = source.Transaction.RefId,
                ExternalGameId = source.Game.GameDetail.GameTable.Id.ToString(),
                ExternalTransactionReferenceId = source.Transaction.RefId,
                GameActionType = ProcessingSettlement(source),
                TimeStamp = DateTimeOffset.UtcNow
            };

            return GetBetData(data, playCode, transactions);
        }

        async Task<BetData> IEvolutionActionModelMapper.CancelAsync(GameProviderConfigurationData data, string playCode, EvolutionCancelRequest source)
        {
            var transactions = default(TransactionData);

            var externalTransactionId = source.Transaction.Id;
            if (source.Transaction.Id != source.Transaction.RefId)
            {
                externalTransactionId = $"{ source.Transaction.Id}_settle";
            }
            var order = await _services.GetGameActionExternalIdAsync(data.GameProviderId, playCode, externalTransactionId).ConfigureAwait(false);
            if (order != null)
            {
                string action = (order.GameActionType == GameActionType.PlaceBet) ? "cancel_bet" : "cancel_settle";
                transactions = new TransactionData()
                {
                    ExternalGameId = source.Game.GameDetail.GameTable.Id.ToString(),
                    ExternalRoundId = source.Game.Id,
                    ExternalBetId = source.Transaction.RefId,
                    ExternalTransactionId = $"{ order.ExternalTransactionId}_{action}",
                    ExternalTransactionReferenceId = order.ExternalTransactionId,
                    GameActionType = GameActionType.CancelTransaction,
                    RoundClosed = false
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
