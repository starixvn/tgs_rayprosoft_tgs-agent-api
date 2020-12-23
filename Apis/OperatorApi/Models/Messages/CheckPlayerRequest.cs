using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record CheckPlayerRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; init; }
    }
}