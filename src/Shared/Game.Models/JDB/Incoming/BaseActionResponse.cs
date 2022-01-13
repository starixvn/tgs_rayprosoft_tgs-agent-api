using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record BaseActionResponse : BaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}
