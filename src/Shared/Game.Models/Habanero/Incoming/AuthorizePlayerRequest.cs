using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record AuthorizePlayerRequest : BaseRequest
    {
        [JsonPropertyName("playerdetailrequest")]
        public PlayerDetail Detail { get; init; }
    }

    public record PlayerDetail
    {
        [JsonPropertyName("token")]
        public string Token { get; init; }

        [JsonPropertyName("gamelaunch")]
        public bool GameLaunch { get; init; }
    }
}