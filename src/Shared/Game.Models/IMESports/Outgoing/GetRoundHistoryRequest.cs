// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record GetRoundHistoryRequest : BaseRequest
    {
        [JsonPropertyName("StartDate")]
        public string StartDate { get; init; }

        [JsonPropertyName("EndDate")]
        public string EndDate { get; init; }

        [JsonPropertyName("Page")]
        public int Page { get; init; }

        [JsonPropertyName("ProductWallet")]
        public int ProductWallet { get; init; }

        [JsonPropertyName("DateFilterType")] public int DateFilterType => 1;

        [JsonPropertyName("Language")]
        public string Language { get; init; }
    }
}