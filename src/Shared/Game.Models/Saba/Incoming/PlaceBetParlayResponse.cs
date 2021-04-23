using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetParlayResponse : BaseResponse
    {
        [JsonPropertyName("txns")]
        public Transaction[] Transactions { get; init; }

        public record Transaction
        {
            [JsonPropertyName("refId")]
            public long ReferenceId { get; init; }

            [JsonPropertyName("licenseeTxId")]
            public long ExternalRoundId { get; init; }
        }
    }
}