using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AsiaGaming.Common;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;
using TransactionType = TGS.Shared.Game.Models.AsiaGaming.Enums.TransactionType;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public sealed class AsiaGamingActionModelMapper : IAsiaGamingActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.AsiaGaming;
        private readonly IAsiaGamingDataStore _store;

        public AsiaGamingActionModelMapper(
            IAsiaGamingDataStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async ValueTask<BetData> CancelTransactionRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            bool isLockGame,
            OppositeBetSetting[] oppositeBetSettings)
        {
            var currencyCode = r.Record.Currency;

            var count = r.Record.TransactionIds.Length;
            var transactions = r.Record.TransactionIds.Select((tId, index) =>
            {
                var transactionType = r.Record.TransactionType;
                var billNo = r.Record.BillNo;

                string transactionId;
                string transactionReferenceId;

                if (isLockGame && !string.IsNullOrEmpty(billNo))
                {
                    transactionId = $"{tId}_{billNo}_{transactionType}";
                    transactionReferenceId = $"{tId}_{billNo}";
                }
                else
                {
                    transactionId = $"{tId}_{transactionType}";
                    transactionReferenceId = $"{tId}";
                }

                return new TransactionData
                {
                    Amount = Math.Abs(r.Record.Amount),
                    ExternalTransactionId = transactionId,
                    ExternalTransactionReferenceId = transactionReferenceId,
                    ExternalRoundId = r.Record.RoundId,
                    ExternalBetId = transactionReferenceId,
                    ExternalGameId = GetGameId(r),
                    GameActionType = GameActionType.CancelTransaction,
                    TimeStamp = GetTimestamp(r.Record.BetTime),
                    RoundClosed = index == count - 1 ? null : false
                };
            }).ToArray();

            foreach (var t in transactions)
            {
                await _store.StoreBetInfoAsync(t.ExternalRoundId, t.ExternalTransactionId, true);
            }

            return GetBetData(playerCode, data, transactions, currencyCode, oppositeBetSettings: oppositeBetSettings);
        }

        public BetData LockAmountRequest(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r)
        {
            var currencyCode = r.Record.Currency;
            var transactionId = r.Record.TransactionIds[0];
            var timestamp = r.Record.BetTime;
            var amount = Math.Abs(r.Record.Amount);

            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = amount,
                    ExternalTransactionId = transactionId,
                    ExternalRoundId = r.Record.RoundId,
                    ExternalBetId = transactionId,
                    ExternalGameId = GetGameId(r),
                    GameActionType = GameActionType.LockAmount,
                    TimeStamp = GetTimestamp(timestamp),
                    RoundDetail = r.Record.GameResult,
                    BetType = r.Record.PlayType
                }
            };

            return GetBetData(playerCode, data, transactions, currencyCode);
        }

        public async ValueTask<BetData> PlaceBetForLockGameRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            BetResponseRequest r,
            GameActionExternalIds lockGameActionExternalIds,
            OppositeBetSetting[] oppositeBetSettings)
        {
            var transactionId = r.TransactionId;

            var transactions = new List<TransactionData>
            {
                new()
                {
                    Amount = Math.Abs(lockGameActionExternalIds.Amount),
                    ExternalTransactionId = $"{transactionId}_{GameActionType.UnlockAmount}",
                    ExternalRoundId = lockGameActionExternalIds.ExternalRoundId,
                    ExternalBetId = transactionId,
                    ExternalGameId = lockGameActionExternalIds.ExternalGameId,
                    GameActionType = GameActionType.UnlockAmount,
                    TimeStamp = lockGameActionExternalIds.RecordedOn
                }
            };

            var betTransactions = r.Records.Select(re => new TransactionData
            {
                Amount = re.Amount,
                ExternalTransactionId = $"{transactionId}_{re.BillNo}",
                ExternalRoundId = lockGameActionExternalIds.ExternalRoundId,
                ExternalBetId = $"{transactionId}_{re.BillNo}",
                ExternalGameId = lockGameActionExternalIds.ExternalGameId,
                GameActionType = GameActionType.PlaceBet,
                TimeStamp = lockGameActionExternalIds.RecordedOn,
                BetType = re.PlayType
            }).ToArray();

            foreach (var t in betTransactions)
            {
                await _store.StoreBetInfoAsync(t.ExternalRoundId, t.ExternalTransactionId, false);
            }

            transactions.AddRange(betTransactions);

            return GetBetData(playerCode, data, transactions.ToArray(),
                hasCurrencyFromGameProvider: false, oppositeBetSettings: oppositeBetSettings);
        }

        public async ValueTask<BetData> PlaceBetRequestAsync(
                    string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            OppositeBetSetting[] oppositeBetSettings)
        {
            var currencyCode = r.Record.Currency;

            var count = r.Record.TransactionIds.Length;
            var transactions = r.Record.TransactionIds.Select((tId, index) =>
            {
                var transactionId = tId;

                return new TransactionData
                {
                    Amount = Math.Abs(r.Record.Amount),
                    ExternalTransactionId = transactionId,
                    ExternalRoundId = r.Record.RoundId,
                    ExternalBetId = transactionId,
                    ExternalGameId = GetGameId(r),
                    GameActionType = GameActionType.PlaceBet,
                    TimeStamp = GetTimestamp(r.Record.BetTime),
                    RoundClosed = index == count - 1 ? null : false,
                    BetType = r.Record.PlayType
                };
            }).ToArray();

            foreach (var t in transactions)
            {
                await _store
                    .StoreBetInfoAsync(t.ExternalRoundId, t.ExternalTransactionId, false)
                    .ConfigureAwait(false);
            }

            return GetBetData(playerCode, data, transactions, currencyCode, oppositeBetSettings: oppositeBetSettings);
        }

        public TransferResponse Response(GameApiError error) =>
            new()
            {
                ResponseCode = error,
                Balance = 0
            };

        public TransferResponse Response(GameApiError error, GetBalanceResult result) =>
            new()
            {
                ResponseCode = error,
                Balance = result?.Balance ?? 0
            };

        public TransferResponse Response(BetResult result) =>
            new()
            {
                ResponseCode = GameApiError.OK,
                Balance = result.Balance
            };

        public async ValueTask<(BetData BetData, bool RoundClosed)> SettleBetRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            TransactionType transactionType,
            bool isLockGame,
            OppositeBetSetting[] oppositeBetSettings)
        {
            var currencyCode = r.Record.Currency;

            var amount = r.Record.BetAmount + r.Record.WinAmount;
            var gameActionType = GetGameActionType(transactionType, amount, r.Record.GameType);

            var isRoundFinish = bool.TryParse(r.Record.RoundFinish, out var finish) && finish;

            var ids = r.Record.TransactionIds.Select(tId =>
            {
                var billNo = r.Record.BillNo;

                var transactionId = isLockGame ?
                    $"{tId}_{billNo}_{transactionType}" :
                    $"{tId}_{transactionType}";

                var transactionReferenceId = isLockGame ? $"{tId}_{billNo}" : tId;

                return new
                {
                    TransactionId = transactionId,
                    TransactionReferenceId = transactionReferenceId
                };
            }).ToArray();

            foreach (var id in ids)
            {
                await _store.StoreBetInfoAsync(r.Record.RoundId, id.TransactionReferenceId, true);
            }

            var betInfos = await _store.GetBetInfosAsync(r.Record.RoundId).ConfigureAwait(false);
            var roundClosed = betInfos.All(info => info.IsSettled);

            var transaction = new TransactionData
            {
                Amount = amount,
                ExternalTransactionId = ids[0].TransactionId,
                ExternalTransactionReferenceId = ids[0].TransactionReferenceId,
                ExternalRoundId = r.Record.RoundId,
                ExternalBetId = ids[0].TransactionReferenceId,
                ExternalGameId = GetGameId(r),
                GameActionType = gameActionType,
                TimeStamp = GetTimestamp(r.Record.SettleTime),
                RoundClosed = roundClosed,
                RoundDetail = GetRoundHistory(isRoundFinish, r.Record.GameResult)
            };

            var betData = GetBetData(playerCode,
                data,
                new[] { transaction },
                currencyCode,
                oppositeBetSettings: oppositeBetSettings);

            return (betData, roundClosed);
        }

        #region Private Methods

        private BetData GetBetData(
            string playerCode,
            GameProviderConfigurationData data,
            TransactionData[] transactions,
            string currencyCode = null,
            bool hasCurrencyFromGameProvider = true,
            OppositeBetSetting[] oppositeBetSettings = null)
            => new()
            {
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = GameProviderCode,
                AutoCloseRound = data.AutoCloseRound,
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                HasCurrencyFromGameProvider = hasCurrencyFromGameProvider,
                CurrencyCode = currencyCode,
                Transactions = transactions,
                OppositeBetRules = oppositeBetSettings?.Select(o => new OppositeBetRule
                {
                    ExternalGameIds = o.Games,
                    Patterns = o.Patterns?.Select(p => new OppositeBetRule.Pattern(p.BetType, p.GroupId)).ToArray()
                }).ToArray()
            };

        private GameActionType GetGameActionType(TransactionType transactionType, decimal payoutAmount, string gameType)
            => transactionType switch
            {
                TransactionType.WIN => GameActionType.WinBet,
                TransactionType.LOSE when payoutAmount > 0 && gameType == "DT" => GameActionType.TieBet,
                TransactionType.LOSE when payoutAmount > 0 => GameActionType.WinBet,
                TransactionType.LOSE => GameActionType.LoseBet,
                TransactionType.REFUND => GameActionType.CancelTransaction,
                _ => GameActionType.PlaceBet
            };

        private string GetGameId(PostTransferRequest r) =>
            $"{r.Record.GamePlatformType}_{r.Record.GameRound}_{r.Record.GameType}";

        private string GetRoundHistory(bool isRoundFinish, string roundResult) =>
            isRoundFinish && roundResult.IsNotNullOrEmpty()
                ? JsonSerializer.Serialize(new GameHistory { RoundResult = roundResult })
                : string.Empty;

        private DateTimeOffset GetTimestamp(string betTime)
        {
            var offset = TimeSpan.FromHours(-4);
            var dateTime = DateTime.ParseExact(betTime, "MM/dd/yyyy HH:mm:sss", null, DateTimeStyles.AllowWhiteSpaces);
            return new(dateTime, offset);
        }

        #endregion Private Methods
    }
}
