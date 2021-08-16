using System.Collections.Generic;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.WMCasino.Enums;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record WalletTransferRequest : BaseRequest
    {
        [JsonPropertyName("user")]
        public string User { get; init; }

        [JsonPropertyName("money")]
        public decimal Amount { get; init; }

        [JsonPropertyName("dealid")]
        public string DealId { get; init; }

        [JsonPropertyName("gtype")]
        public string ExternalGameId { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("betdetail")]
        public IReadOnlyDictionary<string, decimal> BetDetail { get; init; }

        [JsonPropertyName("gameno")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("walletactiontype")]
        public WalletActionType ActionType { get; init; }

        [JsonPropertyName("category")]
        public string Category { get; init; }
    }
}