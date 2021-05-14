using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IsResettled
    {
        NotResettled = 0,
        Resettled = 1
    }
}
