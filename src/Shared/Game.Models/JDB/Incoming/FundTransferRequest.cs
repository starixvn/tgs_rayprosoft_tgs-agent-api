using System;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.JDB.Enums;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record FundTransferRequest : BaseRequest
    {
        [JsonPropertyName("fundtransferrequest")]
        public TransferDetail Detail { get; init; }
    }

    public record TransferDetail
    {
        [JsonPropertyName("token")]
        public string Token { get; init; }

        [JsonPropertyName("accountid")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("customplayertype")]
        public PlayerType PlayerType { get; init; }

        [JsonPropertyName("gameinstanceid")]
        public string RoundId { get; init; }

        [JsonPropertyName("friendlygameinstanceid")]
        public int ClientRoundId { get; init; }

        [JsonPropertyName("isretry")]
        public bool IsRetry { get; init; }

        [JsonPropertyName("retrycount")]
        public int RetryCount { get; init; }

        [JsonPropertyName("isrefund")]
        public bool IsCancel { get; init; }

        [JsonPropertyName("isrecredit")]
        public bool IsSettlementRetry { get; init; }

        [JsonPropertyName("funds")]
        public FundTransfer FundTransfer { get; init; }

        [JsonIgnore]
        public ActionType ActionType
        {
            get
            {
                if (IsCancel)
                {
                    return ActionType.CancelTransaction;
                }

                if (IsSettlementRetry)
                {
                    return ActionType.RetrySettlement;
                }

                if (FundTransfer.IsBetAndSettle)
                {
                    return ActionType.PlaceAndSettleBet;
                }

                var firstTrans = FundTransfer.Transactions[0];
                return firstTrans.IsPlaceBet ? ActionType.PlaceBet :
                    firstTrans.IsFreeSpin    ? ActionType.FreeBet : ActionType.SettleBet;
            }
        }
    }

    public record FundTransfer
    {
        [JsonPropertyName("debitandcredit")]
        public bool IsBetAndSettle { get; init; }

        [JsonPropertyName("fundinfo")]
        public Transaction[] Transactions { get; init; }

        [JsonPropertyName("refund")]
        public CancelTransaction CancelTransaction { get; init; }
    }

    public record Transaction
    {
        [JsonPropertyName("transferid")]
        public string TransactionId { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("currencycode")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("jpwin")]
        public bool IsJackpotWin { get; init; }

        [JsonPropertyName("jpid")]
        public Guid JackpotId { get; init; }

        [JsonPropertyName("jpcont")]
        public decimal JackpotContribution { get; init; }

        [JsonPropertyName("gamestatemode")]
        public GameState GameState { get; init; }

        [JsonPropertyName("dtevent")]
        public string Timestamp { get; init; }

        [JsonPropertyName("initialdebittransferid")]
        public string TransactionReferenceId { get; init; }

        [JsonPropertyName("gameinfeature")]
        public bool IsFreeSpin { get; init; }

        [JsonIgnore]
        public bool IsPlaceBet => Amount < 0;
    }

    public record CancelTransaction
    {
        [JsonPropertyName("gamestatemode")]
        public GameState GameState { get; init; }

        [JsonPropertyName("originaltransferid")]
        public string TransactionReferenceId { get; init; }

        [JsonPropertyName("transferid")]
        public string TransactionId { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("currencycode")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("jpcont")]
        public decimal JackpotContribution { get; init; }

        [JsonPropertyName("dtevent")]
        public string Timestamp { get; init; }
    }
}