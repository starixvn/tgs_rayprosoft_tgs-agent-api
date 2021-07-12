using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetParlayResponse : ErrorResponse
    {
        [JsonPropertyName("txns")]
        public Transaction[] Transactions { get; init; }

        public record Transaction
        {
            [JsonPropertyName("refId")]
            public string ReferenceId { get; init; }

            [JsonPropertyName("licenseeTxId")]
            public string ExternalRoundId { get; init; }
        }
    }
}