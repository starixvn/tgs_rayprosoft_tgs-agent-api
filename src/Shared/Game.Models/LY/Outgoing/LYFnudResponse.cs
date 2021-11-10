using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Shared.Game.Models.LY.Outgoing
{
    public record LYFnudResponse : LYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public string Balance { get; init; }
    }
}
