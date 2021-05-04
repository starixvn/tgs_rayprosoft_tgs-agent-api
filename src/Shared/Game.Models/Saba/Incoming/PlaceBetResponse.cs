using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetResponse : ErrorResponse
    {
        [JsonPropertyName("refId")]
        public long ReferenceId { get; init; }

        [JsonPropertyName("licenseeTxId")]
        public long ExternalRoundId { get; init; }
    }
}