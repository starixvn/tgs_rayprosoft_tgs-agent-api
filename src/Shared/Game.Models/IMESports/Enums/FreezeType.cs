using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FreezeType : short
    {
        SystemLevelFreeze = 8001,
        SystemLevelUnfreeze = 8002,
        MemberLevelFreeze = 8003,
        MemberLevelUnfreeze = 8004
    }
}
