using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record LoginRequest : BaseRequest
    {
        [JsonPropertyName("password")]
        public string Password { get; init; }
    }
}
