using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetResponse : ErrorResponse
    {
        [JsonPropertyName("refId")]
        public string ReferenceId { get; init; }

        [JsonPropertyName("licenseeTxId")]
        public string ExternalRoundId { get; init; }
    }
}