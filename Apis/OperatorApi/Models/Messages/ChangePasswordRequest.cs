using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ChangePasswordRequest : IRequest<ChangePasswordResponse>
    {
        [JsonPropertyName("oldpwd")]
        public string OldPassword { get; set; }

        [JsonPropertyName("newpwd")]
        public string NewPassword { get; set; }

        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; set; }
    }
}