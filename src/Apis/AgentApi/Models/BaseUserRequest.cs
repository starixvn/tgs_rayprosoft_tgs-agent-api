using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models
{
    public abstract record BaseUserRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; init; }
    }
}