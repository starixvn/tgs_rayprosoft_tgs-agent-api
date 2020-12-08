// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record GetRoundHistoryRequest : BaseRequest
    {
        [JsonPropertyName("RefNos")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }
    }
}
