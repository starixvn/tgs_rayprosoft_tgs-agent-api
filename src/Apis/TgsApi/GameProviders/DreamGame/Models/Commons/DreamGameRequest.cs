using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

public record DreamGameRequest
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
