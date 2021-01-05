using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record UpdatePlayerRequest : IRequest<UpdatePlayerResponse>
    {
        [Required]
        [JsonPropertyName("realname")]
        public string RealName { get; init; }

        [Required]
        [JsonPropertyName("phone")]
        public string Phone { get; init; }

        [Required]
        [JsonPropertyName("email")]
        public string Email { get; init; }

        [Required]
        [JsonPropertyName("qq")]
        public string QQ { get; init; }

        [Required]
        [JsonPropertyName("wechat")]
        public string Wechat { get; init; }
    }
}
