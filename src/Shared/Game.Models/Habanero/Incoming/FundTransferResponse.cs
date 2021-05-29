using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Habanero.Enums;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record FundTransferResponse
    {
        [JsonPropertyName("fundtransferresponse")]
        public TransferResult Result { get; init; }
    }

    public record TransferResult
    {
        [JsonPropertyName("status")]
        public FundTransferStatus Status { get; set; }

        [JsonPropertyName("balance")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? Balance { get; set; }

        [JsonPropertyName("currencycode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CurrencyCode { get; set; }
    }

    public record FundTransferStatus : Status
    {
        [JsonPropertyName("nofunds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? HasInsufficientFunds { get; set; }

        [JsonPropertyName("successdebit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? DebitSucceeded { get; set; }

        [JsonPropertyName("successcredit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? CreditSucceeded { get; set; }

        [JsonPropertyName("refundstatus")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public CancelStatus? CancelStatus { get; set; }
    }
}