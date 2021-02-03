using System.Text.Json.Serialization;
using TGS.Core.Models.Services.BetHistories;

namespace TGS.Services.PluggableServices.Models
{
    public record GetRoundHistoryResponse
    {
        [JsonPropertyName("gpname")]
        public string GameProviderName { get; init; }

        [JsonPropertyName("gname")]
        public string GameName { get; init; }

        [JsonPropertyName("externalroundid")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("betamount")]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("turnover")]
        public decimal Turnover { get; init; }

        [JsonPropertyName("winloss")]
        public decimal Winloss { get; init; }

        [JsonPropertyName("result")]
        public string GameResult { get; init; }

        [JsonPropertyName("data")]
        public IData[] Data { get; init; }
    }
}