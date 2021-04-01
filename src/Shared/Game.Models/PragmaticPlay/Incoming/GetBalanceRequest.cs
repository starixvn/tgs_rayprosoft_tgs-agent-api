using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record GetBalanceRequest : BaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }
    }
}