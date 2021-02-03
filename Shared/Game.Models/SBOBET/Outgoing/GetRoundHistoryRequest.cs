// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record GetRoundHistoryRequest : BaseRequest
    {
        [JsonPropertyName("RefNos")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }

        [JsonPropertyName("Language")]
        public string Language { get; init; }
    }
}