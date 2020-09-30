using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record BaseRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; init; }
    }
}