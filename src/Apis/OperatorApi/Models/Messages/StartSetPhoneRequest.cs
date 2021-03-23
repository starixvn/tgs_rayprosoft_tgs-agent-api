using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record StartSetPhoneRequest
    {
        [JsonPropertyName("phone")]
        public string Phone { get; init; }
    }
}
