using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Authentications
{
    public record AuthRequest
    {
        [JsonPropertyName("client")]
        public string ApiKey { get; init;}

        [JsonPropertyName("currency")]
        public string Currency { get; init; }
    }
}
