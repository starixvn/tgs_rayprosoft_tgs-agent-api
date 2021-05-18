using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.IMESports.Enums;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record FreezeRequest : Base
    {
        [JsonPropertyName("ActionId")]
        public ActionType ActionType { get; init; }
        
        [JsonPropertyName("Type")]
        public FreezeType FreezeType { get; init; }
        
        [JsonPropertyName("MemberCodeList")]
        public string[] PlayerCodes { get; init; }
    }
}