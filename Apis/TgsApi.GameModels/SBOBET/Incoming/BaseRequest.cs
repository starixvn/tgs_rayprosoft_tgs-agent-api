using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record BaseRequest
    {
        [JsonPropertyName("companyKey")]
        public string CompanyKey { get; init; }

        [JsonPropertyName("userName")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("productType")]
        public int ProductType { get; init; }

        [JsonPropertyName("gameType")]
        public int GameType { get; init; }

        public string ExternalGameId => $"{ProductType}_{GameType}";
    }
}