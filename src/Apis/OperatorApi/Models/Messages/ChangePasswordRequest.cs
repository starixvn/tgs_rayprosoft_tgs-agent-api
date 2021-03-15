using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ChangePasswordRequest : IRequest<ChangePasswordResponse>
    {
        [Required]
        [JsonPropertyName("oldpwd")]
        public string OldPassword { get; set; }

        [Required]
        [JsonPropertyName("newpwd")]
        public string NewPassword { get; set; }
    }
}