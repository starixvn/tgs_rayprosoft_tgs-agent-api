using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming;

public class ValidateTokenRequest
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
