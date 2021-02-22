// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record GetBetHistoryRequest : BaseRequest
    {
        [JsonPropertyName("RefNos")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }

        [JsonPropertyName("StartDate")]
        public string StartDate { get; init; }

        [JsonPropertyName("EndDate")]
        public string EndDate { get; init; }
    }
}