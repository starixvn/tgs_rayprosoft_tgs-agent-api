using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Shared.Game.Models.LY.Outgoing
{
    public record LYGetBalanceRequest : LYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; init; }
    }
}
