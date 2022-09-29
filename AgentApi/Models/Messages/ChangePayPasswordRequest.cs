using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages;

public class ChangePayPasswordRequest
{
    [JsonPropertyName("newpwd")]
    public string NewPassword { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }
}
