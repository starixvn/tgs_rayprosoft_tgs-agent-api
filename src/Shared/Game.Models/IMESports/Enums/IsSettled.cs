using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IsSettled
    {
        NotSettled = 0,
        Settled = 1
    }
}
