using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record UpdatePlayerRequest : IRequest<UpdatePlayerResponse>
    {
        [JsonPropertyName("realname")]
        public string RealName { get; init; }

        [JsonPropertyName("phone")]
        public string Phone { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; }

        [JsonPropertyName("qq")]
        public string QQ { get; init; }

        [JsonPropertyName("wechat")]
        public string Wechat { get; init; }
    }
}
